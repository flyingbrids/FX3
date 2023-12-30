`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date: 04/15/2022 03:51:28 PM
// Design Name: 
// Module Name: FX3SlaveFIFOInterface
// Project Name: 
// Target Devices: 
// Tool Versions: 
// Description: 
// 
// Dependencies: 
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
//////////////////////////////////////////////////////////////////////////////////
module FX3SlaveFIFOInterface #
(
   parameter integer BUF_NUM = 1,
   parameter logic [4:0] BUF_DEPTH [BUF_NUM-1:0] = {4'd8} // depth is 8 to 15
)
(
        // clk and reset
         input logic fx3_clk,
		 input logic fx3_rst,
		 input logic sys_clk,
		 input logic sys_rst,
		 input logic frame_rst,
		 // FX3 signals
		 input  logic [7:0] fx3_din,
		 output logic [7:0] fx3_dout,
		 input  logic fx3_flaga,
		 output logic[1:0] fx3_a,
		 output logic fx3_slrd_n,
		 output logic fx3_slwr_n, 
		 output logic fx3_sloe_n, 
		 output logic fx3_clk_s,
		 // image data download
		 output logic [79:0] pixDataSync,
		 output logic  pixDatavldSync,
		 input  logic [11:0] imageCol,
		 input  logic [11:0] imageRow,
		 output logic [31:0] downloadByteCnt,
		 output logic bulkInStart,		 
		 input  logic loopRead,
		 output logic [31:0] debugCnt,
		 input  logic Fx3ReadDelay,
		 // RAM write interface from other subsystem
         input logic [BUF_NUM-1:0] buffDataVld,
         input logic [31:0] buffData [BUF_NUM-1:0],
         input logic [BUF_NUM-1:0] buffXerReady,
         output logic[BUF_NUM-1:0] currentXferDone
    );
//---------------------frame rst synchronizer ---------------------------------------
logic  frame_rst_sys, frame_rst_fx3, frame_rst_fx3_fbin, frame_rst_pix_fb;

always @ (posedge sys_clk, posedge sys_rst) begin
      if (sys_rst)
         frame_rst_sys <= 1'b0;
      else if (frame_rst_pix_fb)
         frame_rst_sys <= 1'b0;
      else if (frame_rst)
         frame_rst_sys <= 1'b1;
end

toggle_sync frame_rst_sync_1 (     
       .sig_in(frame_rst_sys),                     
       .clk_b (fx3_clk),      
       .rst_b (fx3_rst),       
       .sig_sync(frame_rst_fx3_fbin), 
       .pulse_sync(frame_rst_fx3) 
);

toggle_sync frame_rst_sync_fb (    
       .sig_in(frame_rst_fx3_fbin),                         
       .clk_b (sys_clk),         
       .rst_b (sys_rst),         
       .pulse_sync(frame_rst_pix_fb) 
);

//---------------------fx3 slavefifo interface---------------------------------------
localparam logic [4:0] LOOPBACK_DEPTH = 11;
logic  [7:0] loopbackData;
logic        loopbackDatavld;
logic  [7:0] imagerData;
logic        imagerDataVld;
logic        bulkInReady;
logic        readPause;
logic  [3:0] switchDone;

// fx3_clk switcher (different clock for bulk in and bulk out)
always @ (posedge fx3_clk, posedge fx3_rst) begin
     if (fx3_rst)
        fx3_clk_s <= 1'b0;
     else if (frame_rst_fx3)
        fx3_clk_s <= 1'b0;
     else if (bulkInReady)
        fx3_clk_s <= 1'b1;
 end
 
always @ (posedge fx3_clk, posedge fx3_rst) begin
     if (fx3_rst)
        switchDone <= '0;
     else if (frame_rst_fx3)
        switchDone <= '0;
     else 
        switchDone <= {switchDone[2:0],fx3_clk_s}; 
end

Fx3DataInterface #
 ( 
   .LOOPBACK_DEPTH(LOOPBACK_DEPTH)
 )
 Fx3DataInterface_i (
         .fx3_clk   (fx3_clk),
		 .fx3_rst   (fx3_rst),
		 .frame_rst_fx3 (frame_rst_fx3),
		 .sys_clk   (sys_clk),
		 .sys_rst   (sys_rst),		 
		 .frame_rst (frame_rst_sys),		 
		 .fx3_din   (fx3_din),
		 .fx3_dout  (fx3_dout),
		 .fx3_flaga (fx3_flaga),
		 .fx3_a     (fx3_a),
		 .fx3_slrd_n(fx3_slrd_n),
		 .fx3_slwr_n(fx3_slwr_n), 
		 .fx3_sloe_n(fx3_sloe_n),		 
		 .loopbackData   (loopbackData),
		 .loopbackDatavld(loopbackDatavld),
		 .pixDataSync    (pixDataSync),
		 .pixDatavldSync (pixDatavldSync),
		 .imagerDataVld  (imagerDataVld),
		 .imagerData     (imagerData),
		 .bulkInReady    (bulkInReady),
		 .imageCol       (imageCol),
		 .imageRow       (imageRow),
		 .downloadByteCnt(downloadByteCnt),
		 .pause          (readPause),
		 .Fx3ReadDelay   (Fx3ReadDelay)
);

//---------------------fx3 data loader--------------------------------------   
logic [BUF_NUM :0] dataInVld;
logic [31:0] dataIn [BUF_NUM :0];
logic [BUF_NUM :0] dataInRdy;
logic [BUF_NUM :0] bufDone;

assign dataInVld = {buffDataVld,loopbackDatavld};
assign dataInRdy = {buffXerReady, &switchDone};
assign dataIn    = {buffData,{24'd0,loopbackData}};

always @ (posedge sys_clk) begin
      currentXferDone = bufDone[BUF_NUM:1];
      bulkInStart = bufDone[0];
end

localparam logic [4:0] BUF_DEPTH_ [BUF_NUM:0] = {BUF_DEPTH,LOOPBACK_DEPTH};
 
 fx3DataLoader #
 ( 
   .BUF_NUM  (BUF_NUM +1),
   .BUF_DEPTH(BUF_DEPTH_)
 )
 fx3DataLoader_i
 (
         .fx3_clk   (fx3_clk),
		 .fx3_rst   (fx3_rst),
		 .frame_rst_fx3 (frame_rst_fx3),
		 .sys_clk   (sys_clk),
		 .sys_rst   (sys_rst),		 
		 .frame_rst (frame_rst_sys),
         .fx3Ready  (bulkInReady),
         .dataInVld (dataInVld),
         .dataIn    (dataIn),
         .dataInRdy (dataInRdy),
         .readPause (readPause),
         .loopRead  (loopRead),  
		 .debugCnt  (debugCnt),
         .outVld    (imagerDataVld),
         .dataOut   (imagerData),
         .bufDone   (bufDone)
 );
    
endmodule
