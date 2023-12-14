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
//**  File Name          : BulkEndpointMarco.h
//**  Module Description : This file contains Macros for bulk EP
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

#ifndef _INCLUDED_CYFXSLFIFOASYNC_H_
#define _INCLUDED_CYFXSLFIFOASYNC_H_

#include "cyu3externcstart.h"
#include "cyu3types.h"
#include "cyu3usbconst.h"


/* 16/32 bit GPIF Configuration select */
/* Set MAIN_APP_GPIF_16_32BIT_CONF_SELECT = 0 for 16 bit GPIF data bus.
 */
#define MAIN_APP_GPIF_16_32BIT_CONF_SELECT (0)

/* set up DMA channel for loopback/short packet/ZLP transfers */
//#define LOOPBACK_SHRT_ZLP

/* set up DMA channel for stream IN/OUT transfers */
#define STREAM_IN_OUT

/* set up MANUAL DMA channel for stream IN/OUT transfers */
//#define MANUAL

#ifdef LOOPBACK_SHRT_ZLP
#define BURST_LEN 1
#define DMA_BUF_SIZE						  (1)
#define MAIN_APP_DMA_BUF_COUNT_P_2_U      (2)              /* Slave FIFO P_2_U channel buffer count - Used with AUTO DMA channel */
#define MAIN_APP_DMA_BUF_COUNT_U_2_P 	  (2)			   /* Slave FIFO U_2_P channel buffer count - Used with AUTO DMA channel */
#endif

#ifdef STREAM_IN_OUT
#define BURST_LEN 16
#define DMA_BUF_SIZE						 (16)
/* Slave FIFO P_2_U channel buffer count */
#define MAIN_APP_DMA_BUF_COUNT_P_2_U      (2)
/* Slave FIFO U_2_P channel buffer count */
#define MAIN_APP_DMA_BUF_COUNT_U_2_P 	  (10)
#endif

#define MAIN_APP_DMA_BUF_COUNT      (2)                   /* Slave FIFO channel buffer count - This is used with MANUAL DMA channel */
#define MAIN_APP_DMA_TX_SIZE        (0)	                  /* DMA transfer size is set to infinite */
#define MAIN_APP_DMA_RX_SIZE        (0)	                  /* DMA transfer size is set to infinite */
#define MAIN_APP_THREAD_STACK       (0x0400)              /* Slave FIFO application thread stack size */
#define MAIN_APP_THREAD_PRIORITY    (8)                   /* Slave FIFO application thread priority */

/* Endpoint and socket definitions for the Slave FIFO application */

/* To change the Producer and Consumer EP enter the appropriate EP numbers for the #defines.
 * In the case of IN endpoints enter EP number along with the direction bit.
 * For eg. EP 6 IN endpoint is 0x86
 *     and EP 6 OUT endpoint is 0x06.
 * To change sockets mention the appropriate socket number in the #defines. */

/* Note: For USB 2.0 the endpoints and corresponding sockets are one-to-one mapped
         i.e. EP 1 is mapped to UIB socket 1 and EP 2 to socket 2 so on */

#define CY_FX_EP_PRODUCER               0x01    /* EP 1 OUT */
#define CY_FX_EP_CONSUMER               0x81    /* EP 1 IN */

#define CY_FX_PRODUCER_USB_SOCKET    CY_U3P_UIB_SOCKET_PROD_1    /* USB Socket 1 is producer */
#define CY_FX_CONSUMER_USB_SOCKET    CY_U3P_UIB_SOCKET_CONS_1    /* USB Socket 1 is consumer */

/* Used with FX3 Silicon. */
// Socket # is the same as thread #
#define CY_FX_PRODUCER_PPORT_SOCKET    CY_U3P_PIB_SOCKET_0    /* P-port Socket 0 is producer */
#define CY_FX_CONSUMER_PPORT_SOCKET    CY_U3P_PIB_SOCKET_3    /* P-port Socket 3 is consumer */

/* Extern definitions for the USB Descriptors */
extern const uint8_t CyFxUSB20DeviceDscr[];
extern const uint8_t CyFxUSB30DeviceDscr[];
extern const uint8_t CyFxUSBDeviceQualDscr[];
extern const uint8_t CyFxUSBFSConfigDscr[];
extern const uint8_t CyFxUSBHSConfigDscr[];
extern const uint8_t CyFxUSBBOSDscr[];
extern const uint8_t CyFxUSBSSConfigDscr[];
extern const uint8_t CyFxUSBStringLangIDDscr[];
extern const uint8_t CyFxUSBManufactureDscr[];
extern const uint8_t CyFxUSBProductDscr[];
extern void BulkInEnumerate ();
extern void BulkOutEnumerate();
extern void BulkEndpointConfigure (uint16_t size, uint8_t burstLength);
extern void BulkEndpointDelete (void);


#include "cyu3externcend.h"

#endif /* _INCLUDED_CYFXSLFIFOASYNC_H_ */

/*[]*/
