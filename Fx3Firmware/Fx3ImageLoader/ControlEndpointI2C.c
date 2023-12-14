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
//**  File Name          : ControlEndpointI2C.c
//**  Module Description : The module configures control EP and I2C transfer functions
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
#include "cyu3i2c.h"
#include "cyu3spi.h"
#include "cyu3uart.h"
#include "ControlEndpointMarco.h"

CyU3PReturnStatus_t
CyFxI2cInit (void)
{
    CyU3PI2cConfig_t i2cConfig;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    /* Initialize and configure the I2C master module. */
    status = CyU3PI2cInit ();
    if (status != CY_U3P_SUCCESS)
    {
        return status;
    }

    /* Start the I2C master block. The bit rate is set at 100KHz.
     * The data transfer is done via DMA. */
    CyU3PMemSet ((uint8_t *)&i2cConfig, 0, sizeof(i2cConfig));
    i2cConfig.bitRate    = CY_FX_USBI2C_I2C_BITRATE;
    i2cConfig.busTimeout = 0xFFFFFFFF;
    i2cConfig.dmaTimeout = 0xFFFF;
    i2cConfig.isDma      = CyFalse;

    status = CyU3PI2cSetConfig (&i2cConfig, NULL);
    return status;
}

/* I2C read / write for programmer application. */
CyU3PReturnStatus_t
CyFxUsbI2cTransfer (
        uint8_t   i2cAddr,
        uint16_t  byteAddress,
        uint8_t   addressCount,
        uint16_t  byteCount,
        uint8_t  *buffer,
        CyBool_t  isRead)
{
    CyU3PI2cPreamble_t preamble;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    if (byteCount == 0)
    {
        return CY_U3P_SUCCESS;
    }

    if (isRead)
        {
            /* Update the preamble information. */
    	   if (addressCount == 1){
    		   preamble.length    = 3;
    		   preamble.buffer[0] = i2cAddr;
    		   preamble.buffer[1] = (uint8_t)(byteAddress & 0xFF);
    		   preamble.buffer[2] = (i2cAddr | 0x01);
    		   preamble.ctrlMask  = 0x0002;
    	   }
    	   else if (addressCount == 2){
    		   preamble.length    = 4;
    		   preamble.buffer[0] = i2cAddr;
    		   preamble.buffer[1] = (uint8_t)((byteAddress >> 8) & 0xFF);
    		   preamble.buffer[2] = (uint8_t)(byteAddress & 0xFF);
    		   preamble.buffer[3] = (i2cAddr | 0x01);
    		   preamble.ctrlMask  = 0x0004;
    	   }

            status = CyU3PI2cReceiveBytes (&preamble, buffer, byteCount, 0);
            if (status != CY_U3P_SUCCESS)
            {
                return status;
            }
        }
     else /* Write */
        {
            /* Update the preamble information. */
    	 if (addressCount == 1){
            preamble.length    = 2;
            preamble.buffer[0] = i2cAddr;
            preamble.buffer[1] = (uint8_t)(byteAddress & 0xFF);
            preamble.ctrlMask  = 0x0000;
    	 }
    	 else if  (addressCount == 2) {
             preamble.length    = 3;
             preamble.buffer[0] = i2cAddr;
  		     preamble.buffer[1] = (uint8_t)((byteAddress >> 8) & 0xFF);
  		     preamble.buffer[2] = (uint8_t)(byteAddress & 0xFF);
             preamble.ctrlMask  = 0x0000;
    	 }
            status = CyU3PI2cTransmitBytes (&preamble, buffer, byteCount, 10);
            if (status != CY_U3P_SUCCESS)
            {
                return status;
            }


            /* Wait for the write to complete. */
            preamble.length = 1;
            status = CyU3PI2cWaitForAck(&preamble, 200);
            if (status != CY_U3P_SUCCESS)
            {
                return status;
            }
        }

        /* An additional delay seems to be required after receiving an ACK. */
       CyU3PThreadSleep (1);

     return CY_U3P_SUCCESS;
}

CyU3PReturnStatus_t
CyFxUsbI2cTransferEnable ()
{
    CyU3PI2cPreamble_t preamble;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
    preamble.length    = 1;
    preamble.buffer[0] = 0x70 << 1;
    preamble.ctrlMask  = 0x0000;
    uint8_t buffer[2];

    // enable Bus 0
    buffer[0] = 0x01;
    status = CyU3PI2cTransmitBytes (&preamble, buffer, 1, 0);
    preamble.length = 1;
    status = CyU3PI2cWaitForAck(&preamble, 100);

    // enable ADM1293
    uint8_t   wValue;
    uint8_t  i2cAddr;
    buffer[0] = 0x20;
    buffer[1] = 0x00;
    for (wValue = 0x31; wValue <= 0x3E; wValue++){
    	i2cAddr = (wValue << 1);
    	CyFxUsbI2cTransfer (i2cAddr,0xD8,1,2,buffer, CyFalse);
    }

    // enable Bus 1
    buffer[0] = 0x02;
    status = CyU3PI2cTransmitBytes (&preamble, buffer, 1, 0);
    preamble.length = 1;
    status = CyU3PI2cWaitForAck(&preamble, 100);

    // enable clock
    CyU3PThreadSleep (100);

    status = enable_AD9576();
    if (status == CY_U3P_SUCCESS)
        status = enable_AD9528();

    return status;
}

void AD9576_register_write (uint16_t registerIndex, uint8_t value) {
	uint8_t i2cAddr;
	i2cAddr = (0x3C << 1);
	uint8_t buffer[1];
	buffer[0] = value;
	CyFxUsbI2cTransfer (i2cAddr,registerIndex,2,1,buffer,CyFalse);
}

CyU3PReturnStatus_t enable_AD9576(){

	// ---------------general configuration -----------------------------
    // 0x40 [1] = 0 to enable chip
	AD9576_register_write (0x40, 0x00);
    // 0x80 [1:0] = 10 XTAL REF0
	AD9576_register_write (0x80, 0x42);

	// ---------------PLL0 configuration -------------------------------
	// pll0 mode 0: fout = fref x2 *(N0+ fra/mod)/(M0Qy)
	//                   = 25*2*(53+23/50)/10/2 = 133.65MHz

	// enable input doubler, PLL0 mode = 00, N0 SDM enable
	AD9576_register_write (0x101, 0x08);
	// leave charge current and RC filter as default
	// N0 = 53 = 0x35
	AD9576_register_write (0x107, 0x35);
	// fra = 335544 * 23 = 0x75C288
	AD9576_register_write (0x108, 0x88);
	AD9576_register_write (0x109, 0xC2);
	AD9576_register_write (0x10A, 0x75);
	// mod = 335544 * 50 = 0xFFFFF0
	AD9576_register_write (0x10B, 0xF0);
	AD9576_register_write (0x10C, 0xFF);
	AD9576_register_write (0x10D, 0xFF);
	// enable M0 disable M1
	AD9576_register_write (0x120, 0x20);
    // M0 = 10
	AD9576_register_write (0x121, 0x0A);

	// ---------------distribution configuration ------------------------
	// configure Q2 -1 = 1
	AD9576_register_write (0x14A, 0x01);
	// feed M0 out to Q2
	AD9576_register_write (0x14B, 0x00);
    // enable OUT6 LVDS
	AD9576_register_write (0x14C, 0x00);

	// ---------------system configuration -----------------------------
    // update IO
	AD9576_register_write (0xF, 0x01);

    // reset VCO M0 divider (automatically done after calibration)

	// Calibrate PLL0 VCO
	AD9576_register_write (0x100, 0x00);
	AD9576_register_write (0xF, 0x01);
	AD9576_register_write (0x100, 0x08);
	AD9576_register_write (0xF, 0x01);

    return CY_U3P_SUCCESS;
}

void AD9528_register_write (uint16_t registerIndex, uint8_t value) {
	uint8_t i2cAddr;
	i2cAddr = (0x54 << 1);
	uint8_t buffer[1];
	buffer[0] = value;
	CyFxUsbI2cTransfer (i2cAddr,registerIndex,2,1,buffer,CyFalse);
}

CyU3PReturnStatus_t enable_AD9528(){
	// --------------------PLL1 configuration------------------------
	// disable hold over
	AD9528_register_write (0x107, 0x20);
	// VCO1 differential input mode
	AD9528_register_write (0x108, 0x01);
	//-------------------- PLL2 configuration------------------------
	// fvco2 = fin * M1 * N2 = 148.5*4*6 = 3564M ;
	// fdist = fin * N2 = 148.5*6 = 891M

	// charge pump current 230*3.5uA
	AD9528_register_write (0x200, 0xE6);
	// Feedback Factor = 4B+A = 24, A=0, B=6
	AD9528_register_write (0x201, 0x06);
	// enable frequency doubler and charge pump current
	AD9528_register_write (0x202, 0x23);
	// enable doubler path, ignore PLL1 validity,
	AD9528_register_write (0x203, 0x14);
	// M1 = 4
	AD9528_register_write (0x204, 0x04);
	// configure RC filter
	AD9528_register_write (0x205, 0x2A);
	// set divider R1 = 1
	AD9528_register_write (0x207, 0x01);
	// N2-1 = 5
	AD9528_register_write (0x208, 0x5);

	//-------------------- clock distribution control-------------------

	for (uint16_t registerIndex = 0x300; registerIndex <= 0x327; registerIndex = registerIndex + 3) {
		// set other configuration as 0s
		AD9528_register_write (registerIndex , 0x00);
		AD9528_register_write (registerIndex+1, 0x00);
		// divisor is 6 for all 14 channels for now
		AD9528_register_write (registerIndex+2 , 0x5);
	}

	//-------------------- system control-----------------------------------
	// sysref control is not applicable

	// update IO
	AD9528_register_write (0xF, 0x01);

	// calibrate PLL2
    AD9528_register_write (0x203, 0x15);
	AD9528_register_write (0xF, 0x01);

	// sync all output
	AD9528_register_write (0x32A, 0x01);
	AD9528_register_write (0xF, 0x01);
	AD9528_register_write (0x32A, 0x00);
	AD9528_register_write (0xF, 0x01);

	return CY_U3P_SUCCESS;
}

CyU3PReturnStatus_t
CyFxUsbI2cTransferFeedback (
		uint8_t  *buffer)
{
    CyU3PI2cPreamble_t preamble;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
    preamble.length    = 1;
    preamble.buffer[0] = (0x70 << 1) + 0x01;
    preamble.ctrlMask  = 0x0000;
    status = CyU3PI2cReceiveBytes (&preamble, buffer, 1, 0);
    return status;
}

