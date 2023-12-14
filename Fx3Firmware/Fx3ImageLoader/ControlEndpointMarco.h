/*
 * ControlEndpointMarco.h
 *
 *  Created on: May 31, 2021
 *      Author: ShenL
 */

#ifndef CONTROLENDPOINTMARCO_H_
#define CONTROLENDPOINTMARCO_H_

#include "cyu3types.h"
#include "cyu3usbconst.h"
#include "cyu3externcstart.h"


CyU3PReturnStatus_t enable_AD9576();
CyU3PReturnStatus_t enable_AD9528();
CyU3PReturnStatus_t CyFxSPIInit();
CyU3PReturnStatus_t CyFxI2cInit();
CyU3PReturnStatus_t CyFxUsbI2cTransferEnable();
CyU3PReturnStatus_t InitImageSensor_FRAMOS();
CyU3PReturnStatus_t SPI_WriteReg (uint16_t regAddr, uint8_t value);
CyU3PReturnStatus_t System_Init (uint16_t masterMode);
CyU3PReturnStatus_t SlaveImageGo();
CyU3PReturnStatus_t MasterImageGo();
CyU3PReturnStatus_t CyFxSpiSetSsnLine1 (CyBool_t isHigh, CyBool_t isSlave);
void AD9528_register_write (uint16_t registerIndex, uint8_t value);

CyU3PReturnStatus_t
CyFxSpiTransfer (
        uint16_t  startAddress, // register offset address
        uint16_t  byteCount,
        uint8_t  *buffer,
        CyBool_t  isRead,
        CyBool_t isSlave);

CyU3PReturnStatus_t
CyImageSensorSpiTransfer (
        uint16_t  startAddress, // register offset address
        uint16_t  byteCount,
        uint8_t  *buffer,
        CyBool_t  isRead,
        CyBool_t isSlave);

CyU3PReturnStatus_t
CyFxSpiTransmitWords (
        uint8_t *data,
        uint32_t byteCount);

CyU3PReturnStatus_t
CyFxSpiReceiveWords (
        uint8_t *data,
        uint32_t byteCount);

CyU3PReturnStatus_t
CyFxUsbI2cTransfer (
        uint8_t   i2cAddr,
        uint16_t  byteAddress,
        uint8_t   addressCount,
        uint16_t  byteCount,
        uint8_t  *buffer,
        CyBool_t  isRead);

/* Give a timeout value of 5s for any flash programming. */
#define CY_FX_USB_SPI_TIMEOUT                   (5000)

/* USB vendor requests supported by the application. */
#define CY_FX_RQT_ID_CHECK           (0xB0)
#define CLK_ENABLE             	     (0xC0)
#define REGISTER_INIT                (0xC1)
#define REGISTER_WRITE               (0xC2)
#define REGISTER_READ                (0xC3)
#define IMAGESNSR_WRITE            	 (0xC4)
#define IMAGESNSR_READ               (0xC5)
#define IMAGESNSR_START              (0xC6)
#define REGISTER_WRITE_SLAVE         (0xC7)
#define REGISTER_READ_SLAVE          (0xC8)
#define IMAGESNSR_WRITE_SLAVE        (0xC9)
#define IMAGESNSR_READ_SLAVE         (0xCA)

/*I2C clock rate is 100kHz. */
#define CY_FX_USBI2C_I2C_BITRATE     (100000)

/* Values for the GPIO */
#define CYFX_GPIO_HIGH           CY_U3P_LPP_GPIO_OUT_VALUE  /* GPIO value is high */

/* GPIO Ids used to */
#define FX3_SPI_CLK             (37) /* GPIO Id 37 will be used for providing SPI Clock */
#define FX3_SPI_SS1             (22) /* GPIO Id 22 will be used as slave select line1 */
#define FX3_SPI_SS              (23) /* GPIO Id 23 will be used as slave select line */
#define FX3_SPI_SS2             (26) /* GPIO Id 26 will be used as slave select line2 */
#define FX3_SPI_SS3             (27) /* GPIO Id 27 will be used as slave select line3 */
#define FX3_SPI_MISO            (24) /* GPIO Id 24 will be used as MISO line */
#define FX3_SPI_MOSI            (25) /* GPIO Id 25 will be used as MOSI line */
#define FPGA_PROGRAM_B          (50) /* GPIO Id 53 will be used as BOOT_EN line */
#define FPGA_INIT_B             (52) /* GPIO Id 53 will be used as BOOT_EN line */
#define FX3_BOOT_EN             (53) /* GPIO Id 53 will be used as BOOT_EN line */
#endif /* CONTROLENDPOINTMARCO_H_ */
