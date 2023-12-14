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
//**  File Name          : cyflxslfifosync
//**  Module Name        : cyflxslfifosync
//**                     :
//**  Module Description : This file configures the sa
//**                     :
//**                     :
//**                     :
//**                     :
//**  Author             :
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
#include "cyu3gpif.h"
#include "cyu3pib.h"
#include "pib_regs.h"
#include <cyu3gpio.h>
CyU3PDmaChannel glChHandleSlFifoUtoP;   /* DMA Channel handle for U2P transfer. */
CyU3PDmaChannel glChHandleSlFifoPtoU;   /* DMA Channel handle for P2U transfer. */


extern void BulkOutEnumerate ()
{
    CyU3PDmaChannelReset (&glChHandleSlFifoUtoP);
    CyU3PUsbFlushEp(CY_FX_EP_PRODUCER);
    CyU3PUsbResetEp (CY_FX_EP_PRODUCER);
    CyU3PDmaChannelSetXfer (&glChHandleSlFifoUtoP, MAIN_APP_DMA_TX_SIZE);
}

extern void BulkInEnumerate (){
    CyU3PDmaChannelReset (&glChHandleSlFifoPtoU);
    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER);
    CyU3PUsbResetEp (CY_FX_EP_CONSUMER);
    CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU, MAIN_APP_DMA_RX_SIZE);
}


extern void BulkEndpointConfigure (uint16_t size, uint8_t burstLength){


    CyU3PEpConfig_t epCfg;
    CyU3PDmaChannelConfig_t dmaCfg;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;
	CyU3PMemSet ((uint8_t *)&epCfg, 0, sizeof (epCfg));
    epCfg.enable = CyTrue;
    epCfg.epType = CY_U3P_USB_EP_BULK;
#ifdef STREAM_IN_OUT
    epCfg.burstLen = burstLength;
#else
    epCfg.burstLen = 1;
#endif
    epCfg.streams = 0;
    epCfg.pcktSize = size;

    /* Producer endpoint configuration */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_PRODUCER, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }

    /* Consumer endpoint configuration */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }

#ifdef MANUAL
    /* Create a DMA MANUAL channel for U2P transfer.
     * DMA size is set based on the USB speed. */
    dmaCfg.size  = DMA_BUF_SIZE* size;
    dmaCfg.count = MAIN_APP_DMA_BUF_COUNT_U_2_P;
    dmaCfg.prodSckId = CY_FX_PRODUCER_USB_SOCKET;
    dmaCfg.consSckId = CY_FX_CONSUMER_PPORT_SOCKET;
    dmaCfg.dmaMode = CY_U3P_DMA_MODE_BYTE;
    /* Enabling the callback for produce event. */
    dmaCfg.notification = CY_U3P_DMA_CB_PROD_EVENT;
    dmaCfg.cb = CyFxSlFifoUtoPDmaCallback;
    dmaCfg.prodHeader = 0;
    dmaCfg.prodFooter = 0;
    dmaCfg.consHeader = 0;
    dmaCfg.prodAvailCount = 0;

    apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoUtoP,
            CY_U3P_DMA_TYPE_MANUAL, &dmaCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Create a DMA MANUAL channel for P2U transfer. */
    dmaCfg.prodSckId = CY_FX_PRODUCER_PPORT_SOCKET;
    dmaCfg.consSckId = CY_FX_CONSUMER_USB_SOCKET;
    dmaCfg.count = MAIN_APP_DMA_BUF_COUNT_P_2_U;
    dmaCfg.cb = CyFxSlFifoPtoUDmaCallback;
    apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoPtoU,
            CY_U3P_DMA_TYPE_MANUAL, &dmaCfg);

#else
    /* Create a DMA AUTO channel for U2P transfer.
       DMA size is set based on the USB speed. */
    dmaCfg.size  = DMA_BUF_SIZE* size;
    dmaCfg.count = MAIN_APP_DMA_BUF_COUNT_U_2_P;
	dmaCfg.prodSckId = CY_FX_PRODUCER_USB_SOCKET;
    dmaCfg.consSckId = CY_FX_CONSUMER_PPORT_SOCKET;
    dmaCfg.dmaMode = CY_U3P_DMA_MODE_BYTE;
    /* Enabling the callback for produce event. */
    dmaCfg.notification = 0;
    dmaCfg.cb = NULL;
    dmaCfg.prodHeader = 0;
    dmaCfg.prodFooter = 0;
    dmaCfg.consHeader = 0;
    dmaCfg.prodAvailCount = 0;

    apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoUtoP,
               CY_U3P_DMA_TYPE_AUTO, &dmaCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
       CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
       CyFxAppErrorHandler(apiRetStatus);
    }

    /* Create a DMA AUTO channel for P2U transfer. */
    //dmaCfg.size  = DMA_BUF_SIZE*size; //increase buffer size for higher performance
    dmaCfg.size = 512;
    dmaCfg.count = MAIN_APP_DMA_BUF_COUNT_P_2_U; // increase buffer count for higher performance
    dmaCfg.prodSckId = CY_FX_PRODUCER_PPORT_SOCKET;
    dmaCfg.consSckId = CY_FX_CONSUMER_USB_SOCKET;
    dmaCfg.cb = NULL;
    apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoPtoU,
               CY_U3P_DMA_TYPE_AUTO, &dmaCfg);

    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

#endif

    /* Flush the Endpoint memory */
    CyU3PUsbFlushEp(CY_FX_EP_PRODUCER);
    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER);

    /* Set DMA channel transfer size. */
    apiRetStatus = CyU3PDmaChannelSetXfer (&glChHandleSlFifoUtoP, MAIN_APP_DMA_TX_SIZE);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PDmaChannelSetXfer Failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }
    apiRetStatus = CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU, MAIN_APP_DMA_RX_SIZE);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PDmaChannelSetXfer Failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

}


extern void
gpif_error_cb(CyU3PPibIntrType cbType, uint16_t cbArg)
{

if(cbType==CYU3P_PIB_INTR_ERROR)
{
    switch (CYU3P_GET_PIB_ERROR_TYPE(cbArg))
    {
        case CYU3P_PIB_ERR_THR0_WR_OVERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR0_WR_OVERRUN");
        break;
        case CYU3P_PIB_ERR_THR1_WR_OVERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR1_WR_OVERRUN");
        break;
        case CYU3P_PIB_ERR_THR2_WR_OVERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR2_WR_OVERRUN");
        break;
        case CYU3P_PIB_ERR_THR3_WR_OVERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR3_WR_OVERRUN");
        break;

        case CYU3P_PIB_ERR_THR0_RD_UNDERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR0_RD_UNDERRUN");
        break;
        case CYU3P_PIB_ERR_THR1_RD_UNDERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR1_RD_UNDERRUN");
        break;
        case CYU3P_PIB_ERR_THR2_RD_UNDERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR2_RD_UNDERRUN");
        break;
        case CYU3P_PIB_ERR_THR3_RD_UNDERRUN:
        CyU3PDebugPrint (4, "CYU3P_PIB_ERR_THR3_RD_UNDERRUN");
        break;

        default:
        CyU3PDebugPrint (4, "No Error :%d\n ",CYU3P_GET_PIB_ERROR_TYPE(cbArg));
            break;
    }
}

}

/* This function stops the slave FIFO loop application. This shall be called
 * whenever a RESET or DISCONNECT event is received from the USB host. The
 * endpoints are disabled and the DMA pipe is destroyed by this function. */
extern void
BulkEndpointDelete (
        void)
{
    CyU3PEpConfig_t epCfg;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;


    /* Flush the endpoint memory */
    CyU3PUsbFlushEp(CY_FX_EP_PRODUCER);
    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER);

    /* Destroy the channel */
    CyU3PDmaChannelDestroy (&glChHandleSlFifoUtoP);
    CyU3PDmaChannelDestroy (&glChHandleSlFifoPtoU);

    /* Disable endpoints. */
    CyU3PMemSet ((uint8_t *)&epCfg, 0, sizeof (epCfg));
    epCfg.enable = CyFalse;

    /* Producer endpoint configuration. */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_PRODUCER, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }

    /* Consumer endpoint configuration. */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }
}
