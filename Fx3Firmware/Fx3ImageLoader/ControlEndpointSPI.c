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
//**  File Name          : ControlEndpointSPI.c
//**  Module Description : The module configures control EP and SPI transfer functions
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
#include "cyu3gpio.h"
#include "cyu3uart.h"
#include "cyu3utils.h"
#include "ControlEndpointMarco.h"

CyBool_t MSB;

/* This function intializes the GPIO module.
   Use GPIOs to implement SPI protocol to communicate the SPI slave device. */
CyU3PReturnStatus_t
CyFxSPIInit (void)
{
    //CyU3PGpioClock_t gpioClock;
    CyU3PGpioSimpleConfig_t gpioConfig;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;

    /* Configure output(SPI_CLOCK).*/
    gpioConfig.outValue    = CyFalse;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_SPI_CLK, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling*/
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
                FX3_SPI_CLK, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Configure output(SPI_SSN3)*/
    gpioConfig.outValue    = CyTrue;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_SPI_SS3, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling*/
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
                FX3_SPI_SS3, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Configure output(SPI_SSN2)*/
    gpioConfig.outValue    = CyTrue;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_SPI_SS2, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling*/
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
                FX3_SPI_SS2, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Configure output(SPI_SSN1)*/
    gpioConfig.outValue    = CyTrue;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_SPI_SS1, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling*/
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
                FX3_SPI_SS1, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Configure output(SPI_SSN)*/
    gpioConfig.outValue    = CyTrue;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_SPI_SS, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling*/
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
                FX3_SPI_SS, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Configure input(MISO)*/
    gpioConfig.outValue    = CyFalse;
    gpioConfig.inputEn     = CyTrue;
    gpioConfig.driveLowEn  = CyFalse;
    gpioConfig.driveHighEn = CyFalse;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_SPI_MISO, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling*/
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
                FX3_SPI_MISO, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Configure output(MOSI) */
    gpioConfig.outValue    = CyFalse;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_SPI_MOSI, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling */
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
                FX3_SPI_MOSI, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Configure output(boot enable) */
    gpioConfig.outValue    = CyFalse;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FX3_BOOT_EN, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling */
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
        		FX3_BOOT_EN, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    CyU3PGpioSetValue (FX3_BOOT_EN, CyFalse); // disable boot

    /* Configure output(FPGA_PROGRAM_B) */
    gpioConfig.outValue    = CyFalse;
    gpioConfig.inputEn     = CyFalse;
    gpioConfig.driveLowEn  = CyTrue;
    gpioConfig.driveHighEn = CyTrue;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig(FPGA_PROGRAM_B, &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling */
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
        		FPGA_PROGRAM_B, apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    CyU3PGpioSetValue (FPGA_PROGRAM_B, CyTrue); // dsiable program


    /* Configure input ( FPGA_INIT_B ) */
    gpioConfig.outValue    = CyFalse;
    gpioConfig.inputEn     = CyTrue;
    gpioConfig.driveLowEn  = CyFalse;
    gpioConfig.driveHighEn = CyFalse;
    gpioConfig.intrMode    = CY_U3P_GPIO_NO_INTR;

    apiRetStatus = CyU3PGpioSetSimpleConfig( FPGA_INIT_B , &gpioConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling */
        CyU3PDebugPrint (4, "CyU3PGpioSetSimpleConfig for GPIO Id %d failed, error code = %d\n",
        		 FPGA_INIT_B , apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    return apiRetStatus;
}

/* This function pulls up/down the SPI Clock line. */
CyU3PReturnStatus_t
CyFxSpiSetClockValue (
        CyBool_t isHigh        /* Cyfalse: Pull down the Clock line,
                                  CyTrue: Pull up the Clock line */
        )
{
    CyU3PReturnStatus_t status;

    status = CyU3PGpioSetValue (FX3_SPI_CLK, isHigh);

    return status;
}

/* This function pulls up/down the slave select line. */
CyU3PReturnStatus_t
CyFxSpiSetSsnLine (
        CyBool_t isHigh,        /* Cyfalse: Pull down the SSN line,
                                  CyTrue: Pull up the SSN line */
        CyBool_t isSlave
        )
{

	MSB = CyTrue;
#ifndef FX3_USE_GPIO_REGS
    CyU3PReturnStatus_t status;

    status = isSlave? CyU3PGpioSetValue (FX3_SPI_SS2, isHigh) : CyU3PGpioSetValue (FX3_SPI_SS, isHigh);

    return status;
#else
    uvint32_t *regPtrSS;
    regPtrSS = &GPIO->lpp_gpio_simple[FX3_SPI_SS];
    if(isHigh)
    {
        *regPtrSS |=CYFX_GPIO_HIGH;
    }
    else
    {
        *regPtrSS&=~CYFX_GPIO_HIGH;
    }

    return CY_U3P_SUCCESS;
#endif

}

CyU3PReturnStatus_t
CyFxSpiSetSsnLine1 (
        CyBool_t isHigh,        /* Cyfalse: Pull down the SSN line,
                                  CyTrue: Pull up the SSN line */
        CyBool_t isSlave
)
{
	MSB = CyFalse;
#ifndef FX3_USE_GPIO_REGS
    CyU3PReturnStatus_t status;

    status = isSlave? CyU3PGpioSetValue (FX3_SPI_SS3, isHigh) : CyU3PGpioSetValue (FX3_SPI_SS1, isHigh);

    return status;
#else
    uvint32_t *regPtrSS;
    regPtrSS = &GPIO->lpp_gpio_simple[FX3_SPI_SS1];
    if(isHigh)
    {
        *regPtrSS |=CYFX_GPIO_HIGH;
    }
    else
    {
        *regPtrSS&=~CYFX_GPIO_HIGH;
    }

    return CY_U3P_SUCCESS;
#endif

}


/* This function transmits the byte to the SPI slave device one bit a time.
   Most Significant Bit is transmitted first.
 */
CyU3PReturnStatus_t
CyFxSpiWriteByte (
        uint8_t data)
{
    uint8_t i = 0;
#ifdef FX3_USE_GPIO_REGS
    CyBool_t value;
    uvint32_t *regPtrMOSI, *regPtrClock;
	regPtrMOSI = &GPIO->lpp_gpio_simple[FX3_SPI_MOSI];
	regPtrClock = &GPIO->lpp_gpio_simple[FX3_SPI_CLK];
#endif
    for (i = 0; i < 8; i++)
    {
#ifndef FX3_USE_GPIO_REGS
        /* Most significant bit is transferred first. */
    	if (MSB)
           CyU3PGpioSetValue (FX3_SPI_MOSI, ((data >> (7 - i)) & 0x01));
    	else
    	   CyU3PGpioSetValue (FX3_SPI_MOSI, ((data >> i) & 0x01));
        CyFxSpiSetClockValue (CyTrue);
        CyU3PBusyWait (1);
        CyFxSpiSetClockValue (CyFalse);
        CyU3PBusyWait (1);
#else
        /* Most significant bit is transferred first. */
		value =((data >> (7 - i)) & 0x01);
		if(value)
		{
			*regPtrMOSI |=	CYFX_GPIO_HIGH;
		}
		else
		{
			*regPtrMOSI &=~CYFX_GPIO_HIGH;
		}
		*regPtrClock|=CYFX_GPIO_HIGH;
        CyU3PBusyWait (1);
        *regPtrClock&=~CYFX_GPIO_HIGH;
        CyU3PBusyWait (1);
#endif
    }

    return CY_U3P_SUCCESS;
}

/* This function receives the byte from the SPI slave device one bit at a time.
   Most Significant Bit is received first.
 */
CyU3PReturnStatus_t
CyFxSpiReadByte (
        uint8_t *data)
{
    uint8_t i = 0;
    CyBool_t temp = CyFalse;

#ifdef FX3_USE_GPIO_REGS
    uvint32_t *regPtrClock;
	regPtrClock = &GPIO->lpp_gpio_simple[FX3_SPI_CLK];
#endif
    *data = 0;

    for (i = 0; i < 8; i++)
    {
#ifndef FX3_USE_GPIO_REGS
        CyFxSpiSetClockValue (CyTrue);

        CyU3PGpioGetValue (FX3_SPI_MISO, &temp);
        if (MSB)
          *data |= (temp << (7 - i));
        else
          *data |= (temp << i);

        CyU3PBusyWait (1);
        CyFxSpiSetClockValue (CyFalse);
        CyU3PBusyWait (1);

#else
        *regPtrClock|=CYFX_GPIO_HIGH;
		temp = (GPIO->lpp_gpio_simple[FX3_SPI_MISO] & CY_U3P_LPP_GPIO_IN_VALUE)>>1;
        *data |= (temp << (7 - i));
        CyU3PBusyWait (1);
        *regPtrClock&=~CYFX_GPIO_HIGH;
        CyU3PBusyWait (1);
#endif
    }

    return CY_U3P_SUCCESS;
}

/* This function is used to transmit data to the SPI slave device. The function internally
   calls the CyFxSpiWriteByte function to write to the slave device.
 */
CyU3PReturnStatus_t
CyFxSpiTransmitWords (
        uint8_t *data,
        uint32_t byteCount
)
{
    uint32_t i = 0;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    if ((!byteCount) || (!data))
    {
        return CY_U3P_ERROR_BAD_ARGUMENT;
    }

    for (i = 0; i < byteCount; i++)
    {
        status = CyFxSpiWriteByte (data[i]);

        if (status != CY_U3P_SUCCESS)
        {
            break;
        }
    }

    return status;
}

/* This function is used receive data from the SPI slave device. The function internally
   calls the CyFxSpiReadByte function to read data from the slave device.
 */
CyU3PReturnStatus_t
CyFxSpiReceiveWords (
        uint8_t *data,
        uint32_t byteCount)
{
    uint32_t i = 0;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    if ((!byteCount) || (!data))
    {
        return CY_U3P_ERROR_BAD_ARGUMENT;
    }

    for (i = 0; i < byteCount; i++)
    {
        status = CyFxSpiReadByte (&data[i]);

        if (status != CY_U3P_SUCCESS)
        {
            break;
        }
    }

    return status;
}

/* SPI read / write for programmer application. */
CyU3PReturnStatus_t
CyFxSpiTransfer (
        uint16_t  startAddress, // register offset address
        uint16_t  byteCount,
        uint8_t  *buffer,
        CyBool_t  isRead,
        CyBool_t  isSlave)
{
       uint8_t overHead[1];
       CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
       overHead[0] = startAddress & 0xFF;

        if (isRead)
        {
            overHead[0] = (overHead[0] << 1) + 0x01;  /* Read command. */

            CyFxSpiSetSsnLine (CyFalse,isSlave);
            status = CyFxSpiTransmitWords (overHead, 1);
            if (status != CY_U3P_SUCCESS)
            {
                CyU3PDebugPrint (2, "SPI READ command failed\r\n");
                CyFxSpiSetSsnLine (CyTrue,isSlave);
                return status;
            }

            status = CyFxSpiReceiveWords (buffer, byteCount);
            if (status != CY_U3P_SUCCESS)
            {
                CyFxSpiSetSsnLine (CyTrue,isSlave);
                return status;
            }

            CyFxSpiSetSsnLine (CyTrue,isSlave);
        }
        else /* Write */
        {
        	overHead[0] = overHead[0] << 1; /* Write command */

            CyFxSpiSetSsnLine (CyFalse,isSlave);
            status = CyFxSpiTransmitWords (overHead, 1);
            if (status != CY_U3P_SUCCESS)
            {
                CyU3PDebugPrint (2, "SPI WRITE command failed\r\n");
                CyFxSpiSetSsnLine (CyTrue,isSlave);
                return status;
            }

            status = CyFxSpiTransmitWords (buffer, byteCount);
            if (status != CY_U3P_SUCCESS)
            {
                CyFxSpiSetSsnLine (CyTrue,isSlave);
                return status;
            }

            CyFxSpiSetSsnLine (CyTrue,isSlave);
        }

    CyU3PThreadSleep (5);
    return CY_U3P_SUCCESS;
}

CyU3PReturnStatus_t FPGA_register_write (uint8_t Address, uint8_t *buffer) {
	   uint8_t data[4];
	   CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
	   for (int i =0; i<4; i++)
		   data[i] = buffer[3-i];
	   status = CyFxSpiTransfer (Address, 4, data, CyFalse, CyFalse);
	   status = CyFxSpiTransfer (Address, 4, data, CyFalse, CyTrue);
	   return status;
}

CyU3PReturnStatus_t System_Init (uint16_t masterMode) {
	CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
	uint8_t FPGARegisterBuff[4];
	InitImageSensor_FRAMOS(); // 12 bit mode, line width: 3840

	// init the FRAMOS receiver IP
	FPGARegisterBuff[3] = 0x0;
	FPGARegisterBuff[2] = 0x0;
	FPGARegisterBuff[1] = 0x0;
	FPGARegisterBuff[0] = 0xC;
	status = FPGA_register_write (0x60, FPGARegisterBuff); // 12 bit mode

	FPGARegisterBuff[3] = 0x0;
    FPGARegisterBuff[2] = 0x0;
	FPGARegisterBuff[1] = 0xF;
	FPGARegisterBuff[0] = 0x0;
	status = FPGA_register_write (0x61, FPGARegisterBuff); //  line width: 3840

	FPGARegisterBuff[3] = 0x0;
    FPGARegisterBuff[2] = 0x0;
	FPGARegisterBuff[1] = 0x0;
	FPGARegisterBuff[0] = 0x1;
	status = FPGA_register_write (0x63, FPGARegisterBuff); //  CRC enable

	// reset related imager clock
	FPGARegisterBuff[3] = 0x0;
	FPGARegisterBuff[2] = 0x0;
	FPGARegisterBuff[1] = 0x0;
	FPGARegisterBuff[0] = 0x0;
	status = FPGA_register_write (0x01, FPGARegisterBuff);

	FPGARegisterBuff[3] = 0x0;
	FPGARegisterBuff[2] = 0x0;
	FPGARegisterBuff[1] = 0x0;
	FPGARegisterBuff[0] = 0x1;
	status = FPGA_register_write (0x01, FPGARegisterBuff);

	return status;
}

CyU3PReturnStatus_t ImageGo() {

	CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
	uint8_t FPGARegisterBuff[4];

	FPGARegisterBuff[3] = 0x12;
    FPGARegisterBuff[2] = 0xB2;
	FPGARegisterBuff[1] = 0xF;
	FPGARegisterBuff[0] = 0xD1;
	status = FPGA_register_write (0x69, FPGARegisterBuff); //  H period and VMAX setup

	FPGARegisterBuff[3] = 0x00;
    FPGARegisterBuff[2] = 0x00;
	FPGARegisterBuff[1] = 0x00;
	FPGARegisterBuff[0] = 0x32;
	status = FPGA_register_write (0x6C, FPGARegisterBuff); //  start line

	// set image size
	FPGARegisterBuff[3] = 0x09;
    FPGARegisterBuff[2] = 0x90;
	FPGARegisterBuff[1] = 0x08;
	FPGARegisterBuff[0] = 0x00;
	status = FPGA_register_write (0x03, FPGARegisterBuff); // 2448*2048

	// new frame pulse
	FPGARegisterBuff[3] = 0x0;
    FPGARegisterBuff[2] = 0x0;
	FPGARegisterBuff[1] = 0x0;
	FPGARegisterBuff[0] = 0x0;
	status = FPGA_register_write (0x1, FPGARegisterBuff);

	FPGARegisterBuff[3] = 0x0;
    FPGARegisterBuff[2] = 0x0;
	FPGARegisterBuff[1] = 0x0;
	FPGARegisterBuff[0] = 0x2;
	status = FPGA_register_write (0x1, FPGARegisterBuff);

	return status;
}
