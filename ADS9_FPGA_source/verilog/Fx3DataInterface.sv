`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: Liqing Shen 
// 
// Create Date: 04/14/2022 03:45:12 PM
// Design Name: 
// Module Name: Fx3DataInterface
// Project Name: 
// Target Devices:  
// Tool Versions: 
// Description: This module downloads image data and upload image processing result
// 
// Dependencies: 
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
//////////////////////////////////////////////////////////////////////////////////
module Fx3DataInterface #
(
   parameter integer LOOPBACK_DEPTH = 10
)
(
        // clk and reset
         input logic fx3_clk,
         input logic fx3_rst,
		 input logic frame_rst_fx3,
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
		 // download data
		 output logic [7:0] loopbackData,
		 output logic loopbackDatavld,
		 output logic [79:0] pixDataSync,
		 output logic  pixDatavldSync,
		 // upload data 
		 input  logic imagerDataVld,
		 input  logic [7:0] imagerData,
		 // flow control
		 input  logic [11:0] imageCol,
		 input  logic [11:0] imageRow, 
		 output logic [31:0] downloadByteCnt,
		 output logic bulkInReady,
		 output logic pause,
		 input  logic Fx3ReadDelay
    );
    
logic fx3_reset;
assign fx3_reset = fx3_rst | frame_rst_fx3;

logic sys_reset;
assign sys_reset = sys_rst | frame_rst;

logic [2:0] currentFx3State;
localparam fx3Idle         = 3'd0;
localparam streamOutStart  = 3'd1;
localparam streamInStart   = 3'd3; 

 // flag valid check. When switching thread, delay 2clk before set flagVld signal
 logic       flagVld;
 logic [1:0] fx3_a_d;
 logic [1:0] threadDlyCount;
 logic       threadSwitched;
 assign      threadSwitched = &threadDlyCount;
 always @ (posedge fx3_clk, posedge fx3_reset) begin
      if (fx3_reset) begin
	      fx3_a_d <= '0;
		  threadDlyCount <= '0;
		  flagVld <= '0;
	  end else begin
		  if (fx3_a != fx3_a_d) begin
		     flagVld        <= '0;
			 fx3_a_d        <= threadSwitched? fx3_a : fx3_a_d;
		     threadDlyCount <= threadSwitched? '0    : threadDlyCount + 1'b1;
		  end else begin
			 flagVld <= '1;
			 fx3_a_d <= fx3_a;
			 threadDlyCount <= '0;		
	      end 
	  end 
 end 
  
 // FX3 thread selection. Thread 0 for FPGA->Fx3, Thread 3 for FX3->FPGA 
logic bulkOutDone;
assign bulkInReady = bulkOutDone & threadSwitched; // it is a pulse @fx3_clk domain
always @ (posedge fx3_clk, posedge fx3_reset) begin
      if (fx3_reset) begin
	     fx3_a <= 2'b11;
	  end else if (currentFx3State == fx3Idle & bulkOutDone) begin
	     fx3_a <= 2'b00;
	  end
end
 
// sync fx3 databus
logic fx3_flaga_d;
(*keep ="true" *) logic [7:0]fx3_din_asyn;
(*keep ="true" *) logic [7:0]fx3_din_sync;
logic [7:0]imagerDataDly[3:0];
always @ (posedge fx3_clk, posedge fx3_reset) begin
      if (fx3_reset) begin
		fx3_flaga_d <= 1'b0;
		fx3_din_asyn <= 8'd0;
	    fx3_din_sync <= 8'd0;	
	    imagerDataDly <= '{default: '0};
	  end else begin
		fx3_flaga_d <= flagVld & fx3_flaga;
		fx3_din_asyn <= fx3_din;
	    fx3_din_sync <= fx3_din_asyn;	
	    imagerDataDly[0] <= imagerData;
	    for (int i=1; i<=3; i++)
	        imagerDataDly[i] <= imagerDataDly[i-1];
	  end
end

assign fx3_dout = imagerDataDly[2];

// handle slrd_n to FIFO write & FIFO read to slwr_n latency 
logic  readFx3;
(*keep ="true" *) logic fx3_slrd_n_d;
assign fx3_slrd_n   = ~fx3_flaga_d | bulkOutDone ; 
assign fx3_slrd_n_d = ~fx3_flaga_d | bulkOutDone ; 
assign readFx3 = (currentFx3State == streamOutStart);
logic [2:0]slrd;
logic [2:0]slwr;  
assign fx3_sloe_n = slwr[0]; // set false path to fx3_db
always @ (posedge fx3_clk, posedge fx3_reset) begin
      if (fx3_reset) begin
	     slrd <= '1;
		 slwr <= '0;
		 fx3_slwr_n <= 1'b1;
	 end else begin
	     slrd <= {slrd[1:0],fx3_slrd_n_d};
		 slwr <= (currentFx3State == streamInStart)? {slwr[1:0], 1'b1} : '0;
		 fx3_slwr_n <= !slwr[0]; 
	 end
end

// upload byte count 
logic [8:0] byteCount;
always @ (posedge fx3_clk, posedge fx3_reset) begin
     if (fx3_reset) begin
        byteCount <= '0;
        pause <= 1'b1;
     end else if (imagerDataVld) begin
        byteCount <= byteCount + 1'b1;
        if (byteCount == 9'd510) // Since each fx3 data buf is 512 bytes, needs to pause the bulk out data to allow time for fx3 commit the buffer
            pause <= 1'b1;
     end else if (fx3_flaga_d & (currentFx3State == fx3Idle)& bulkOutDone) begin
        byteCount <= '0;
        pause <= 1'b0;
     end
end

// Fx3 read byte counter
always @ (posedge fx3_clk, posedge fx3_reset) begin
     if (fx3_reset) begin
	     downloadByteCnt <= 32'd0;
     end else if (readFx3) begin
	     downloadByteCnt <= downloadByteCnt + 1; 
     end  
end

// loopback data for signal integrity test
localparam loopbackCnt = 2**LOOPBACK_DEPTH;
always @ (posedge fx3_clk, posedge fx3_reset) begin
     if (fx3_reset) begin
	     loopbackData   <= 8'd0;
		 loopbackDatavld <= 1'b0;
	  end else if (readFx3 & downloadByteCnt < loopbackCnt) begin
		 loopbackData <= fx3_din_sync;
		 loopbackDatavld <= 1'b1;  
	  end else begin
	     loopbackData <= 8'd0;
		 loopbackDatavld <= 1'b0;
	  end
end

// fill out image buffer which buffer at least 1 row of image 
logic [79:0] pixData;
logic  pixDatavld;
always @ (posedge fx3_clk, posedge fx3_reset) begin
     if (fx3_reset) begin
	     pixData <= 32'd0;
		 pixDatavld <= '0;
	  end else if (readFx3) begin
	     case (downloadByteCnt[3:0])
 	     4'd1: begin
				pixData[77:70] <= fx3_din_sync;		  
		  end
 	     4'd0: begin
				pixData[79:78] <= fx3_din_sync[1:0];
		  end
 	     4'd3: begin
				pixData[67:60] <= fx3_din_sync;  
		  end
 	     4'd2: begin
				pixData[69:68] <= fx3_din_sync[1:0];	  
		  end
 	     4'd5: begin
				pixData[57:50] <= fx3_din_sync;	  
		  end
 	     4'd4: begin
				pixData[59:58] <= fx3_din_sync[1:0];	  
		  end
 	     4'd7: begin
				pixData[47:40] <= fx3_din_sync;	  
		  end
 	     4'd6: begin
				pixData[49:48] <= fx3_din_sync[1:0];	  
		  end
 	     4'd9: begin
				pixData[37:30] <= fx3_din_sync;		  
		  end
 	     4'd8: begin
				pixData[39:38] <= fx3_din_sync[1:0];	  
		  end
 	     4'd11: begin
				pixData[27:20] <= fx3_din_sync;	  
		  end
 	     4'd10: begin
				pixData[29:28] <= fx3_din_sync[1:0];  
		  end
 	     4'd13: begin
				pixData[17:10] <= fx3_din_sync;
		  end
 	     4'd12: begin
				pixData[19:18] <= fx3_din_sync[1:0];	  
		  end
 	     4'd15: begin
				pixData[7:0] <= fx3_din_sync;	  
		  end
 	     4'd14: begin
				pixData[9:8] <= fx3_din_sync[1:0];	  
		  end		
		 endcase
		 pixDatavld <= &(downloadByteCnt[3:0]);
     end else begin
	     pixData <= '0;
		 pixDatavld <= '0;
     end
end	

logic line_full;  // set false path 
logic fifo_read;
logic empty;
image_fifo fx3_image_buffer (
  .din       (pixData   )
 ,.wr_en     (pixDatavld)
 ,.prog_full_thresh(imageCol[11:3]-1'b1)
 ,.prog_full (line_full) 
 ,.dout      (pixDataSync)
 ,.rd_en     (fifo_read)
 ,.empty     (empty)
 ,.wr_clk    (fx3_clk)
 ,.rd_clk    (sys_clk)
 ,.rst       (sys_reset)
); 

logic [8:0] col_cnt;
logic [11:0] row_cnt;
always @ (posedge sys_clk, posedge sys_reset) begin
      if (sys_reset) begin
         pixDatavldSync <= 1'b0;
         fifo_read <= 1'b0; 
         col_cnt <= 9'd1; 
         row_cnt <= 12'd1;
		 bulkOutDone <= '0;
      end else begin
         pixDatavldSync <= fifo_read;
         
         if (line_full & ~empty)
            fifo_read <= 1'b1;
         else if (col_cnt == imageCol[11:3])
            fifo_read <= 1'b0;
         
         if (fifo_read) begin
            col_cnt <= (col_cnt == imageCol[11:3])? 9'd1 : col_cnt + 1'b1;      
            row_cnt <= (col_cnt == imageCol[11:3])? row_cnt+1'b1 : row_cnt;
         end   
		 
		 if ((row_cnt == imageRow) & (col_cnt == imageCol[11:3]))
		    bulkOutDone <= '1;
      end
end

logic readStart, readEnd;

assign readStart = (slrd == '0);
assign readEnd   = fx3_slrd_n_d; 

// statemachine 
always @ (posedge fx3_clk, posedge fx3_reset) begin
      if (fx3_reset) begin
	     currentFx3State <= fx3Idle;
	  end else begin
	     currentFx3State <= currentFx3State;
	     case (currentFx3State)
		  fx3Idle: begin
		     if (imagerDataVld)
		         currentFx3State <= streamInStart;	
		     else if (readStart) 
			     currentFx3State <= streamOutStart;			 
		  end
          streamOutStart : begin
			  if (readEnd)
			     currentFx3State <= fx3Idle;	 
		  end
		  streamInStart: begin
		      if (!fx3_flaga_d)
			     currentFx3State <= fx3Idle;	
		  end
		  default: currentFx3State <= fx3Idle;			  
		  endcase
	  end
end 

//debug interface   
//`ifdef DEBUG
ila_0 fx3dataIF (
   .clk (fx3_clk),
   .probe0 (frame_rst_fx3),
   .probe1 (fx3_din_sync),
   .probe2 (readFx3),
   .probe3 (downloadByteCnt),
   .probe4 (currentFx3State),
   .probe5 (fx3_slwr_n),
   .probe6 (fx3_sloe_n),
   .probe7 (fx3_din),
   .probe8 (fx3_slrd_n_d),
   .probe9 (fx3_flaga),
   .probe10 (fx3_dout)           
);
//`endif
  
endmodule
