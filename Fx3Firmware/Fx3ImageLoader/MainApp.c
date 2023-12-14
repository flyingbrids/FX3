//*******************************************************************************************
//**.........................................................................................
//**...000000000000000000.......000000..........000000000000000...........0000000000.........
//**...000000000000000000......00000000.........00000000000000000.......00000000000000.......
//**...000000000000000000.....0000000000........000000000000000000....000000000000000000.....
//**...000000000000000000....:0000000000:.......0000000000000000000..00000000000000000000....
//**...00000000.............:000000000000:......00000000....0000000.000000000....000000000...
//**...000000000000000000...000000..000000......00000000....0000000.00000000.......0000000...
//**...000000000000000000..:00000...0000000.....00000000...0000000..0000000........0000000...
//**...000000000000000000..........................................................0000000...
//**...000000000000000000.:0000000..0000000:....0000000000000000....0000000........0000000...
//**...00000000...........0000000....0000000....00000000.00000000...000000000....000000000...
//**...00000000..........:000000:....0000000:...00000000..00000000...0000000000000000000.....
//**...00000000..........000000:.....:0000000...00000000...00000000...000000000000000000.....
//**...00000000.........:000000.......0000000:..00000000...000000000....00000000000000.......
//**...00000000.........000000.........0000000..00000000....000000000.....0000000000.........
//**.........................................................................................
//*******************************************************************************************
//**
//**  File Name          : MainApp.c
//**  Module Description : This file configures the RTOS thread, USB events and IO matrix
//**                     :
//**                     :
//**                     :
//**                     :
//**                     :
//**                     :
//**  Author             : Liqing Shen
//**  Email              :
//**  Phone              :
//**                     :
//**  Creation Date      :
//**                     :
//**  Version History    :
//**                     :
//**
//*******************************************************************************************

#include "cyu3system.h"
#include "cyu3os.h"
#include "cyu3dma.h"
#include "cyu3error.h"
#include "cyu3usb.h"
#include "cyu3uart.h"
#include "BulkEndpointMarco.h"
#include "ControlEndpointMarco.h"
#include "cyu3gpif.h"
#include "cyu3pib.h"
#include "pib_regs.h"
#include <cyu3gpio.h>

/* This file should be included only once as it contains
 * structure definitions. Including it in multiple places
 * can result in linker error. */
#include "cyfxgpif2config.h"

// FX3 revision
const uint8_t glFirmwareID[32] __attribute__ ((aligned (32))) = { 'F', 'X', '3', ' ', '0', '0', '1', '\0' };

// Control endpoint Buffer
uint8_t glEp0Buffer[4096] __attribute__ ((aligned (32)));

void gpif_error_cb(CyU3PPibIntrType cbType, uint16_t cbArg);

// Main App parameter
CyU3PThread appThread;	                  /* APP thread structure */
//uint32_t glDMARxCount = 0;               /* Counter to track the number of buffers received from USB. */
//uint32_t glDMATxCount = 0;               /* Counter to track the number of buffers sent to USB. */
CyBool_t glIsApplnActive = CyFalse;      /* Whether the application is active or not. */
uint8_t burstLength = 0;


/* This function starts the main APP This is called
 * when a SET_CONF event is received from the USB host. The endpoints
 * are configured and the DMA pipe is setup in this function. */
void
MainAppStart (
        void)
{
	uint16_t size = 0;
    CyU3PUSBSpeed_t usbSpeed = CyU3PUsbGetSpeed();

    /* First identify the usb speed. Once that is identified,
     * create a DMA channel and start the transfer on this. */

    /* Based on the Bus Speed configure the endpoint packet size */
    switch (usbSpeed)
    {
        case CY_U3P_FULL_SPEED:
            size = 64;
            break;

        case CY_U3P_HIGH_SPEED:
            size = 512;
            burstLength=1;

            break;

        case  CY_U3P_SUPER_SPEED:
            size = 1024;
            burstLength=16;
            break;

        default:
            CyU3PDebugPrint (4, "Error! Invalid USB speed.\n");
            CyFxAppErrorHandler (CY_U3P_ERROR_FAILURE);
            break;
    }

    // enumerate the bulk EP
    BulkEndpointConfigure(size,burstLength);

    /* Update the status flag. */
    glIsApplnActive = CyTrue;
}

void
MainAppStop (
        void)
{
    /* Update the flag. */
    glIsApplnActive = CyFalse;

    // delete the bulk EP
    BulkEndpointDelete();

}

/* Callback to handle the USB setup requests. */
CyBool_t
MainAppUSBSetupCB (
        uint32_t setupdat0,
        uint32_t setupdat1
    )
{
    /* Fast enumeration is used. Only requests addressed to the interface, class,
     * vendor and unknown control requests are received by this function.
     * This application does not support any class or vendor requests. */

    uint8_t  bRequest, bReqType, i2cAddr;
    uint8_t  bType, bTarget;
    uint16_t wValue, wIndex, wLength;
    CyBool_t isHandled = CyFalse;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    /* Decode the fields from the setup request. */
    bReqType = (setupdat0 & CY_U3P_USB_REQUEST_TYPE_MASK);
    bType    = (bReqType & CY_U3P_USB_TYPE_MASK);
    bTarget  = (bReqType & CY_U3P_USB_TARGET_MASK);
    bRequest = ((setupdat0 & CY_U3P_USB_REQUEST_MASK) >> CY_U3P_USB_REQUEST_POS);
    wValue   = ((setupdat0 & CY_U3P_USB_VALUE_MASK)   >> CY_U3P_USB_VALUE_POS);
    wIndex   = ((setupdat1 & CY_U3P_USB_INDEX_MASK)   >> CY_U3P_USB_INDEX_POS);
    wLength   = ((setupdat1 & CY_U3P_USB_LENGTH_MASK)   >> CY_U3P_USB_LENGTH_POS);

    if (bType == CY_U3P_USB_STANDARD_RQT)
    {
        /* Handle SET_FEATURE(FUNCTION_SUSPEND) and CLEAR_FEATURE(FUNCTION_SUSPEND)
         * requests here. It should be allowed to pass if the device is in configured
         * state and failed otherwise. */
        if ((bTarget == CY_U3P_USB_TARGET_INTF) && ((bRequest == CY_U3P_USB_SC_SET_FEATURE)
                    || (bRequest == CY_U3P_USB_SC_CLEAR_FEATURE)) && (wValue == 0))
        {
            if (glIsApplnActive)
                CyU3PUsbAckSetup ();
            else
               CyU3PUsbStall (0, CyTrue, CyFalse);

            isHandled = CyTrue;
        }

        /* CLEAR_FEATURE request for endpoint is always passed to the setup callback
         * regardless of the enumeration model used. When a clear feature is received,
         * the previous transfer has to be flushed and cleaned up. This is done at the
         * protocol level. Since this is just a loopback operation, there is no higher
         * level protocol. So flush the EP memory and reset the DMA channel associated
         * with it. If there are more than one EP associated with the channel reset both
         * the EPs. The endpoint stall and toggle / sequence number is also expected to be
         * reset. Return CyFalse to make the library clear the stall and reset the endpoint
         * toggle. Or invoke the CyU3PUsbStall (ep, CyFalse, CyTrue) and return CyTrue.
         * Here we are clearing the stall. */
        if ((bTarget == CY_U3P_USB_TARGET_ENDPT) && (bRequest == CY_U3P_USB_SC_CLEAR_FEATURE)
                && (wValue == CY_U3P_USBX_FS_EP_HALT))
        {
            if (glIsApplnActive)
            {
                if (wIndex == CY_FX_EP_PRODUCER)
                {
                	BulkOutEnumerate();
                }

                if (wIndex == CY_FX_EP_CONSUMER)
                {
                	BulkInEnumerate();
                }

                CyU3PUsbStall (wIndex, CyFalse, CyTrue);
                isHandled = CyTrue;
            }
        }
    }

    /* Handle supported vendor requests.  */
      if (bType == CY_U3P_USB_VENDOR_RQT)
      {
          isHandled = CyTrue;

          switch (bRequest)
          {
             case CY_FX_RQT_ID_CHECK:
                  CyU3PUsbSendEP0Data (8, (uint8_t *)glFirmwareID);
                  break;

             case CLK_ENABLE:
            	  status = CyU3PUsbGetEP0Data (wLength, glEp0Buffer, NULL);
            	  if (status == CY_U3P_SUCCESS)
            	  {
            		  status = enable_AD9576();
            		  if (status == CY_U3P_SUCCESS)
            			  enable_AD9528();
            	  }
                  break;

              case REGISTER_INIT:
            	  status = CyU3PUsbGetEP0Data (wLength, glEp0Buffer, NULL);
            	  if (status == CY_U3P_SUCCESS)
            	      status = System_Init(wIndex);
              break;

              case IMAGESNSR_START :
            	  status = CyU3PUsbGetEP0Data (wLength, glEp0Buffer, NULL);
            	  if (status == CY_U3P_SUCCESS){
            		  ImageGo();
            	  }
              break;

              case REGISTER_WRITE:
                  status = CyU3PUsbGetEP0Data (wLength, glEp0Buffer, NULL);
                  if (status == CY_U3P_SUCCESS)
                  {
                	 status = CyFxSpiTransfer (wIndex, wLength, glEp0Buffer, CyFalse, CyFalse);
                  }
                  break;

              case REGISTER_READ:
                  CyU3PMemSet (glEp0Buffer, 0, sizeof (glEp0Buffer));
                  status = CyFxSpiTransfer (wIndex, wLength, glEp0Buffer, CyTrue, CyFalse);
                  if (status == CY_U3P_SUCCESS)
                  {
                      status = CyU3PUsbSendEP0Data (wLength, glEp0Buffer);
                  }
                  break;

              case IMAGESNSR_WRITE:
            	  i2cAddr = (uint8_t)((wValue & 0x00ff) << 1);
                  status = CyU3PUsbGetEP0Data (wLength, glEp0Buffer, NULL);
                  if (status == CY_U3P_SUCCESS)
                  {
                      status = CyImageSensorSpiTransfer (wIndex, wLength, glEp0Buffer, CyFalse, CyFalse);
                  }
                  break;

              case IMAGESNSR_READ:
                  CyU3PMemSet (glEp0Buffer, 0, sizeof (glEp0Buffer));
                  i2cAddr = (uint8_t)((wValue & 0x00ff) << 1);
                  status = CyImageSensorSpiTransfer (wIndex, wLength, glEp0Buffer, CyTrue, CyFalse);
                  if (status == CY_U3P_SUCCESS)
                  {
                      status = CyU3PUsbSendEP0Data (wLength, glEp0Buffer);
                  }
                  break;

              case REGISTER_WRITE_SLAVE:
                  status = CyU3PUsbGetEP0Data (wLength, glEp0Buffer, NULL);
                  if (status == CY_U3P_SUCCESS)
                  {
                	 status = CyFxSpiTransfer (wIndex, wLength, glEp0Buffer, CyFalse, CyTrue);
                  }
                  break;

              case REGISTER_READ_SLAVE:
                  CyU3PMemSet (glEp0Buffer, 0, sizeof (glEp0Buffer));
                  status = CyFxSpiTransfer (wIndex, wLength, glEp0Buffer, CyTrue, CyTrue);
                  if (status == CY_U3P_SUCCESS)
                  {
                      status = CyU3PUsbSendEP0Data (wLength, glEp0Buffer);
                  }
                  break;

              case IMAGESNSR_WRITE_SLAVE:
            	  i2cAddr = (uint8_t)((wValue & 0x00ff) << 1);
                  status = CyU3PUsbGetEP0Data (wLength, glEp0Buffer, NULL);
                  if (status == CY_U3P_SUCCESS)
                  {
                      status = CyImageSensorSpiTransfer (wIndex, wLength, glEp0Buffer, CyFalse, CyTrue);
                  }
                  break;

              case IMAGESNSR_READ_SLAVE:
                  CyU3PMemSet (glEp0Buffer, 0, sizeof (glEp0Buffer));
                  i2cAddr = (uint8_t)((wValue & 0x00ff) << 1);
                  status = CyImageSensorSpiTransfer (wIndex, wLength, glEp0Buffer, CyTrue, CyTrue);
                  if (status == CY_U3P_SUCCESS)
                  {
                      status = CyU3PUsbSendEP0Data (wLength, glEp0Buffer);
                  }
                  break;

              default:
                  /* This is unknown request. */
                  isHandled = CyFalse;
                  break;
          }
      }


    return isHandled;
}

/* This is the callback function to handle the USB events. */
void
MainAppUSBEventCB (
    CyU3PUsbEventType_t evtype,
    uint16_t            evdata
    )
{
    switch (evtype)
    {
        case CY_U3P_USB_EVENT_SETCONF:
            /* Stop the APP before re-starting. */
            if (glIsApplnActive)
            {
                MainAppStop ();
            }
            CyU3PUsbLPMDisable();
            /* Start the APP. */
            MainAppStart ();
            break;

        case CY_U3P_USB_EVENT_RESET:
        case CY_U3P_USB_EVENT_DISCONNECT:
            /* Stop the APP. */
            if (glIsApplnActive)
            {
                MainAppStop ();
            }
            break;

        default:
            break;
    }
}

/* Callback function to handle LPM requests from the USB 3.0 host. This function is invoked by the API
   whenever a state change from U0 -> U1 or U0 -> U2 happens. If we return CyTrue from this function, the
   FX3 device is retained in the low power state. If we return CyFalse, the FX3 device immediately tries
   to trigger an exit back to U0.

   This application does not have any state in which we should not allow U1/U2 transitions; and therefore
   the function always return CyTrue.
 */
CyBool_t
CyFxApplnLPMRqtCB (
        CyU3PUsbLinkPowerMode link_mode)
{
    return CyTrue;
}


/* This function initializes the GPIF interface and initializes
 * the USB interface. */
void
MainAppInit (void)
{
    CyU3PPibClock_t pibClock;
    CyU3PGpioClock_t gpioClock;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;

    /* Initialize the p-port block. */
    pibClock.clkDiv = 4;
    pibClock.clkSrc = CY_U3P_SYS_CLK;
    pibClock.isHalfDiv = CyFalse;

    /* enable DLL for sync GPIF */
    pibClock.isDllEnable = CyTrue;
    apiRetStatus = CyU3PPibInit(CyTrue, &pibClock);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "P-port Initialization failed, Error Code = %d\n",apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Load the GPIF configuration for Slave FIFO sync mode. */
    apiRetStatus = CyU3PGpifLoad (&CyFxGpifConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PGpifLoad failed, Error Code = %d\n",apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

#if(MAIN_APP_GPIF_16_32BIT_CONF_SELECT == 1)
    CyU3PGpifSocketConfigure (0,CY_U3P_PIB_SOCKET_0,6,CyFalse,1);
    CyU3PGpifSocketConfigure (3,CY_U3P_PIB_SOCKET_3,6,CyFalse,1);
#else
    CyU3PGpifSocketConfigure (0,CY_FX_PRODUCER_PPORT_SOCKET,3,CyFalse,1);
    CyU3PGpifSocketConfigure (3,CY_FX_CONSUMER_PPORT_SOCKET,3,CyFalse,1);
#endif

    /* Start the state machine. */
    apiRetStatus = CyU3PGpifSMStart (RESET,ALPHA_RESET);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PGpifSMStart failed, Error Code = %d\n",apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }


     /* Init the GPIO module*/
	gpioClock.fastClkDiv = 2;
	gpioClock.slowClkDiv = 0;
	gpioClock.simpleDiv = CY_U3P_GPIO_SIMPLE_DIV_BY_2;
	gpioClock.clkSrc = CY_U3P_SYS_CLK;
	gpioClock.halfDiv = 0;

	apiRetStatus = CyU3PGpioInit(&gpioClock, NULL);
	if (apiRetStatus != 0)
	{
		/* Error Handling*/
		CyU3PDebugPrint (4, "CyU3PGpioInit failed, error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

    /* Init GPIO SPI module */
     apiRetStatus = CyFxSPIInit();

     /* Init I2C module*/
     apiRetStatus = CyFxI2cInit();

     /* Configure the resource for FPGA */
     apiRetStatus = CyFxUsbI2cTransferEnable(); // this enable I2C bus and enable voltage regulator

    /* Start the USB functionality. */
    apiRetStatus = CyU3PUsbStart();
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PUsbStart failed to Start, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }
    /* callback to see if there is any overflow of data on the GPIF II side*/
    CyU3PPibRegisterCallback(gpif_error_cb,0xffff);

    /* The fast enumeration is the easiest way to setup a USB connection,
     * where all enumeration phase is handled by the library. Only the
     * class / vendor requests need to be handled by the application. */
    CyU3PUsbRegisterSetupCallback(MainAppUSBSetupCB, CyTrue);

    /* Setup the callback to handle the USB events. */
    CyU3PUsbRegisterEventCallback(MainAppUSBEventCB);

    /* Register a callback to handle LPM requests from the USB 3.0 host. */
    CyU3PUsbRegisterLPMRequestCallback(CyFxApplnLPMRqtCB);    

    /* Set the USB Enumeration descriptors */

    /* Super speed device descriptor. */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_SS_DEVICE_DESCR, NULL, (uint8_t *)CyFxUSB30DeviceDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set device descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* High speed device descriptor. */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_HS_DEVICE_DESCR, NULL, (uint8_t *)CyFxUSB20DeviceDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set device descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* BOS descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_SS_BOS_DESCR, NULL, (uint8_t *)CyFxUSBBOSDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set configuration descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Device qualifier descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_DEVQUAL_DESCR, NULL, (uint8_t *)CyFxUSBDeviceQualDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set device qualifier descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Super speed configuration descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_SS_CONFIG_DESCR, NULL, (uint8_t *)CyFxUSBSSConfigDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set configuration descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* High speed configuration descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_HS_CONFIG_DESCR, NULL, (uint8_t *)CyFxUSBHSConfigDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB Set Other Speed Descriptor failed, Error Code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Full speed configuration descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_FS_CONFIG_DESCR, NULL, (uint8_t *)CyFxUSBFSConfigDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB Set Configuration Descriptor failed, Error Code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* String descriptor 0 */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_STRING_DESCR, 0, (uint8_t *)CyFxUSBStringLangIDDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set string descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* String descriptor 1 */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_STRING_DESCR, 1, (uint8_t *)CyFxUSBManufactureDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set string descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* String descriptor 2 */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_STRING_DESCR, 2, (uint8_t *)CyFxUSBProductDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set string descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Connect the USB Pins with super speed operation enabled. */
    //apiRetStatus = CyU3PConnectState(CyTrue, CyTrue); // The devKit cannot be connected as USB SS device
    apiRetStatus = CyU3PConnectState(CyTrue, CyFalse);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB Connect failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }
}

/* Entry function for the slFifoAppThread. */
void
MainAppThread_Entry (
        uint32_t input)
{
    /* Initialize the debug module */
    MainAppDebugInit();

    /* Initialize the slave FIFO application */
    MainAppInit();

    for (;;)
    {

    	CyU3PThreadSleep (1000);
        if (glIsApplnActive)
        {
            /* Print the number of buffers received so far from the USB host.
            CyU3PDebugPrint (6, "Data tracker: buffers received: %d, buffers sent: %d.\n",
                    glDMARxCount, glDMATxCount);*/
        }
    }
}

/* Application define function which creates the threads. */
void
CyFxApplicationDefine (
        void)
{
    void *ptr = NULL;
    uint32_t retThrdCreate = CY_U3P_SUCCESS;

    /* Allocate the memory for the thread */
    ptr = CyU3PMemAlloc (MAIN_APP_THREAD_STACK);

    /* Create the thread for the application */
    retThrdCreate = CyU3PThreadCreate (&appThread,                 /* main app thread structure */
                          "Fx3 Imager Loader",                    /* Thread ID and thread name */
                          MainAppThread_Entry,                   /* Slave FIFO app thread entry function */
                          0,                                       /* No input parameter to thread */
                          ptr,                                     /* Pointer to the allocated thread stack */
                          MAIN_APP_THREAD_STACK,               /* App Thread stack size */
                          MAIN_APP_THREAD_PRIORITY,            /* App Thread priority */
                          MAIN_APP_THREAD_PRIORITY,            /* App Thread pre-emption threshold */
                          CYU3P_NO_TIME_SLICE,                     /* No time slice for the application thread */
                          CYU3P_AUTO_START                         /* Start the thread immediately */
                          );

    /* Check the return code */
    if (retThrdCreate != 0)
    {
        /* Thread Creation failed with the error code retThrdCreate */

        /* Add custom recovery or debug actions here */

        /* Application cannot continue */
        /* Loop indefinitely */
        while(1);
    }
}

/*
 * Main function
 */
int
main (void)
{
    CyU3PIoMatrixConfig_t io_cfg;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
    CyU3PSysClockConfig_t clkCfg;

        /* setSysClk400 clock configurations */
        clkCfg.setSysClk400 = CyFalse;   /* FX3 device's master clock is set to default: 384MHz*/
        clkCfg.cpuClkDiv = 2;           /* CPU clock divider */
        clkCfg.dmaClkDiv = 2;           /* DMA clock divider */
        clkCfg.mmioClkDiv = 2;          /* MMIO clock divider */
        clkCfg.useStandbyClk = CyFalse; /* device has no 32KHz clock supplied */
        clkCfg.clkSrc = CY_U3P_SYS_CLK; /* Clock source for a peripheral block  */

    /* Initialize the device */
    status = CyU3PDeviceInit (&clkCfg);
    if (status != CY_U3P_SUCCESS)
    {
        goto handle_fatal_error;
    }

    /* Initialize the caches. Enable instruction cache and keep data cache disabled.
     * The data cache is useful only when there is a large amount of CPU based memory
     * accesses. When used in simple cases, it can decrease performance due to large 
     * number of cache flushes and cleans and also it adds to the complexity of the
     * code. */
    status = CyU3PDeviceCacheControl (CyTrue, CyFalse, CyFalse);
    if (status != CY_U3P_SUCCESS)
    {
        goto handle_fatal_error;
    }

    /* IO matrix Initialization */
    io_cfg.useUart   = CyFalse;
    io_cfg.useI2C    = CyTrue;
    io_cfg.useI2S    = CyFalse;
    io_cfg.useSpi    = CyFalse;
    io_cfg.s0Mode 	 = CY_U3P_SPORT_INACTIVE;
    io_cfg.s1Mode 	 = CY_U3P_SPORT_INACTIVE;
#if (MAIN_APP_GPIF_16_32BIT_CONF_SELECT == 0)
    io_cfg.isDQ32Bit = CyFalse;
    io_cfg.lppMode   = CY_U3P_IO_MATRIX_LPP_I2S_ONLY  ;
#else
    io_cfg.isDQ32Bit = CyTrue;
    io_cfg.lppMode   = CY_U3P_IO_MATRIX_LPP_DEFAULT;
#endif
    /* Use GPIOs are SPI interface */
    io_cfg.gpioSimpleEn[0]  = (1 << FX3_SPI_MOSI)+ (1 <<  FX3_SPI_SS) + (1 << FX3_SPI_MISO) + (1<<FX3_SPI_SS1) + (1<<FX3_SPI_SS2) + (1<<FX3_SPI_SS3);
    io_cfg.gpioSimpleEn[1]  = (1 << (FX3_SPI_CLK-32)) + (1 << (FX3_BOOT_EN - 32)) + (1 << (FPGA_PROGRAM_B - 32)) + (1 << (FPGA_INIT_B- 32));
    io_cfg.gpioComplexEn[0] = 0;
    io_cfg.gpioComplexEn[1] = 0;
    status = CyU3PDeviceConfigureIOMatrix (&io_cfg);
    if (status != CY_U3P_SUCCESS)
    {
        goto handle_fatal_error;
    }

    /* This is a non returnable call for initializing the RTOS kernel */
    CyU3PKernelEntry ();

    /* Dummy return to make the compiler happy */
    return 0;

handle_fatal_error:

    /* Cannot recover from this error. */
    while (1);
}

/* [ ] */

