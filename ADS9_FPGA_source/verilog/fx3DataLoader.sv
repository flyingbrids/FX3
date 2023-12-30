`timescale 1ns / 1ps
//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: Liqing Shen
// 
// Create Date: 04/18/2022 03:46:43 PM
// Design Name: 
// Module Name: fx3DataLoader
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
module fx3DataLoader #
(
   parameter integer BUF_NUM = 1,
   parameter logic [4:0] BUF_DEPTH [BUF_NUM-1:0] = {4'd9} 
)
(
  input logic sys_clk,
  input logic sys_rst,
  input logic frame_rst,
  input logic fx3_clk,
  input logic fx3_rst,
  input logic frame_rst_fx3,
  input logic fx3Ready,
  input logic [BUF_NUM-1:0] dataInVld,
  input logic [31:0] dataIn [BUF_NUM-1:0],
  input logic [BUF_NUM-1:0] dataInRdy,
  output logic[BUF_NUM-1:0] bufDone, 
  input logic loopRead,
  input logic readPause,
  output logic outVld,
  output logic [7:0] dataOut,
  output logic [31:0] debugCnt 
);

logic [1:0] dataLoadState;
localparam dataLoadIdle   = 2'd0;
localparam dataLoadWait   = 2'd1;
localparam dataLoadActive = 2'd3;
localparam dataLoadUpdate = 2'd2;

logic fx3_reset;
assign fx3_reset = fx3_rst | frame_rst_fx3;

logic sys_reset;
assign sys_reset = sys_rst | frame_rst;


logic [$clog2(BUF_NUM)-1:0] ramSel;
logic [$clog2(BUF_NUM)-1:0] ramSel_d;
logic [31:0]   RAM_q [BUF_NUM-1:0];
logic [24:0]  inAddr [BUF_NUM-1:0];
logic [24:0] cinAddr [BUF_NUM-1:0];
logic [24:0] outAddr [BUF_NUM-1:0];
logic [1:0]  addrPre [BUF_NUM-1:0];

always @ (posedge fx3_clk, posedge fx3_reset) begin
       if (fx3_reset) begin
          ramSel <= '0;
          ramSel_d <= '0;
       end else if (dataLoadState == dataLoadUpdate) begin 
          ramSel <= (ramSel < BUF_NUM-1)? (ramSel + 1'b1) : (loopRead? 1 : ramSel); 
          ramSel_d <= ramSel;
       end 
end

logic [BUF_NUM-1:0] dataInRdy_sync;
assign dataInRdy_sync[0] = dataInRdy[0];

logic endofRead;
assign endofRead = (ramSel == 0) ? (outAddr[0] == cinAddr[0] - 1) 
                                 : (&(addrPre[ramSel]) & (outAddr[ramSel] == cinAddr[ramSel]));

always @ (posedge fx3_clk, posedge fx3_reset) begin
       if (fx3_reset)
          dataLoadState <= dataLoadIdle;
       else begin
         case (dataLoadState)
         dataLoadIdle: begin
             if (fx3Ready) // just a pause
                 dataLoadState <= dataLoadWait;
         end
         dataLoadWait: begin
             if (dataInRdy_sync[ramSel])
                 dataLoadState <= dataLoadActive;         
         end
         dataLoadActive: begin
             if (endofRead)
                dataLoadState <= dataLoadUpdate; 
         end
         dataLoadUpdate: begin
             if (ramSel < BUF_NUM-1 | loopRead)
                 dataLoadState <= dataLoadWait;
             else 
                 dataLoadState <= dataLoadIdle;
         end
         default: dataLoadState <= dataLoadIdle;
         endcase 
      end
 end

logic buffUpdate;
logic buffUpdateState;

always @ (posedge fx3_clk, posedge fx3_reset) begin
       if (fx3_reset)
          buffUpdateState <= '0;
       else 
          buffUpdateState <= (dataLoadState == dataLoadUpdate)? 1'b1: 1'b0;
end

toggle_sync  buffUpdate_sync (     
       .sig_in(buffUpdateState),                     
       .clk_b (sys_clk),      
       .rst_b (sys_rst),       
       .pulse_sync (buffUpdate)  
);

// loopback buffer 
assign cinAddr[0] = 2**BUF_DEPTH[0];
assign addrPre[0] = 2'b11;
always @ (posedge fx3_clk, posedge fx3_reset) begin
   if (fx3_reset) begin
       inAddr [0] <= '0;
       outAddr[0] <= '0;
   end else begin
      if (dataInVld[0])
         inAddr[0]  <= inAddr [0] + 1'b1;
      if (~readPause & (dataLoadState == dataLoadActive) & (ramSel==0))
         outAddr[0] <= outAddr[0] + 1'b1;     
   end
end

always @ (posedge sys_clk) 
   bufDone[0] <= (ramSel_d == 0) & buffUpdate ? 1'b1 : 1'b0;

dp_bram 
#(  .DATA_WIDTH (8)
   ,.ADDR_WIDTH (BUF_DEPTH[0]) 
)
loopbackBuf
(  .clk_a  (fx3_clk)
  ,.clk_b  (fx3_clk)     
  ,.we_a   (dataInVld[0])
  ,.data_a (dataIn[0][7:0])      
  ,.addr_a (inAddr [0][BUF_DEPTH[0]-1:0])
  ,.addr_b (outAddr[0][BUF_DEPTH[0]-1:0])
  ,.q_b    (RAM_q[0][7:0])
);

// upload buffer
genvar i;
generate
for (i=1; i<BUF_NUM; i++) 
  begin : upload_buffers

    toggle_sync dataRdy_fx3_sync (
       .sig_in(dataInRdy[i]),                    
       .clk_b (fx3_clk),      
       .rst_b (fx3_reset), 
	   .sig_sync (dataInRdy_sync[i])
    );
    
    assign cinAddr[i] = (inAddr[i] < 128)? 25'd128 : {inAddr[i][24:7]+ |inAddr[i][6:0], 7'd0}; //round up to multiple of 128 wors (512 bytes for each fx3 buf)
    
    always @ (posedge sys_clk, posedge sys_reset) begin
       if (sys_reset) 
          inAddr[i] <= '0;
	   else if (bufDone[i])
  	      inAddr[i] <= '0;
       else if (dataInVld[i])
          inAddr[i] <= inAddr[i] + 1'b1;    
    end
    
    always @ (posedge fx3_clk, posedge fx3_reset) begin
       if (fx3_reset) begin
          addrPre[i] <= '0;
          outAddr[i] <= '0;
	   end else if (ramSel != i) begin
          addrPre[i] <= '0;
          outAddr[i] <= '0;	   
       end else if (~readPause & (dataLoadState == dataLoadActive)) begin
          addrPre[i] <= addrPre[i] + 1'b1;
          outAddr[i] <= (addrPre[i] == 2'd2)? outAddr[i] + 1'b1 : outAddr[i];    
       end
    end
	
    always @ (posedge sys_clk) 
        bufDone[i] <= (ramSel_d == i) & buffUpdate ? 1'b1 : 1'b0;
   
      dp_bram 
      #( .DATA_WIDTH (32)
        ,.ADDR_WIDTH (BUF_DEPTH[i]) 
       )
      dp_bram_i
       (  .clk_a  (sys_clk)
         ,.clk_b  (fx3_clk)     
         ,.we_a   (dataInVld[i])
         ,.data_a (dataIn[i])      
         ,.addr_a (inAddr[i] [BUF_DEPTH[i]-1:0])
         ,.addr_b (outAddr[i][BUF_DEPTH[i]-1:0])
         ,.q_b    (RAM_q[i])
       );
  end : upload_buffers
endgenerate

// output     
logic [7:0] dataOutBuf;
always @ (posedge fx3_clk) begin
  case (addrPre[ramSel])
     2'b00: dataOutBuf <= RAM_q[ramSel][31:24];
     2'b01: dataOutBuf <= RAM_q[ramSel][23:16];
     2'b10: dataOutBuf <= RAM_q[ramSel][15:8];
     2'b11: dataOutBuf <= RAM_q[ramSel][7:0];
     default: dataOutBuf <= '0;
  endcase
end

always @ (posedge fx3_clk, posedge fx3_reset) begin
       if (fx3_reset) begin
	      debugCnt <= '0;
	   end else if (outVld) begin
          debugCnt <= debugCnt + 1'b1;
	   end
end	   

`ifdef DEBUG
ila_4 fx3RAM_IF (
  .clk (fx3_clk),
  .probe0 (dataLoadState),
  .probe1 (ramSel),
  .probe2 (debugCnt),
  .probe3 (dataInRdy_sync)
);    
`endif 

assign dataOut = (ramSel == '0) ? RAM_q[0][7:0] : 
                 ((outAddr[ramSel] > inAddr[ramSel]) | ((outAddr[ramSel] == inAddr[ramSel]) & (addrPre[ramSel][0]!=addrPre[ramSel][1])))? '0: dataOutBuf;

always @ (posedge fx3_clk)
     outVld <= (dataLoadState == dataLoadActive)? ~readPause: 1'b0;

endmodule
