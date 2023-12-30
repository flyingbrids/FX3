//////////////////////////////////////////////////////////////////////////////////
// Company: 
// Engineer: 
// 
// Create Date: 04/26/2022 
// Design Name: Leon Shen
// Module Name: 
// Project Name: FX3GPIF
// Target Devices: ADS9-V2EBZ 
// Tool Versions: Vivado 2021.2
// Description: 
// Dependencies: 
// 
// Revision:
// Revision 0.01 - File Created
// Additional Comments:
// 
//////////////////////////////////////////////////////////////////////////////////
`timescale 1ns / 1ps

module FX3GPIF_TOP( 
     // clock resource
     input logic AD9576_OUT6_n,
     input logic AD9576_OUT6_p,
     output logic baseClkforAD9528,
     // FX3 
     output logic fx3_slcs_n,
     input  logic  fx3_flaga,
     output logic [1:0] fx3_a,
     inout  logic [7:0] fx3_db,
     output logic fx3_slrd_n,
     output logic fx3_slwr_n,		  
     output logic fx3_sloe_n,
     input  logic fx3_pclk, 
     input  logic new_frame_req,
     // LED 
     output logic [5:0] led
);

 // --------------clock and reset generation ---------------------------------- 
 logic sys_clk, sys_rst, sys_rst_n, main_pll_lock;     
       
clk_wiz_0 master_clk
(
  .clk_in1_p (AD9576_OUT6_p), // 133.65MHz
  .clk_in1_n (AD9576_OUT6_n), 
  .clk_out1  (baseClkforAD9528), // 74.25MHz, for gigabit transceiver clock chip on board
  .clk_out2  (sync_clk_out), // 133.65MHz
  .clk_out3  (sys_clk), // 167.0625MHz
  .locked    (main_pll_lock)    
);

logic fx3_clk, fx3_out_clk, fx3_in_clk, fx3_lock, fx3_clk_s;
clk_wiz_2 fx3_PLL
(
  .clk_in1   (fx3_pclk), // 96MHz
  .clk_out1  (fx3_out_clk), // 96MHz shifted
  .clk_out2  (fx3_in_clk), // 96MHz 
  .locked    (fx3_lock)    
);

BUFGMUX_CTRL FX3_clk_switcher
(
    .O(fx3_clk), // 1-bit output: Clock output
    .I0(fx3_out_clk), // 1-bit input: Clock input (S=0)
    .I1(fx3_in_clk), // 1-bit input: Clock input (S=1)
    .S(fx3_clk_s) // 1-bit input: Clock select
);

//double flip-flop to sync reset to its associated clock
reset_sync                                    
reset_sync_sys                                
  ( .pll_clk               ( sys_clk       )  
  , .pll_lock              ( main_pll_lock )  
  , .external_rst          ( 1'b0          )  
  , .sync_rst_out          ( sys_rst       )  
  , .sync_rst_out_n        ( sys_rst_n     )
  ) ;   

logic fx3_rst, fx3_rst_n;    
reset_sync                                    
reset_sync_fx3                               
  ( .pll_clk               ( fx3_clk       )  
  , .pll_lock              ( fx3_lock      )  
  , .external_rst          ( ~main_pll_lock)  
  , .sync_rst_out          ( fx3_rst       )  
  , .sync_rst_out_n        ( fx3_rst_n     )
  ) ;   
    
//clock status LEDs   
assign led[0] = main_pll_lock;
assign led[1] = fx3_lock;

localparam SYS_FREQ  = 167062500;
localparam FX3_FREQ  =  96000000;

heartBeatPulse #(.FREQ(SYS_FREQ)) 
sys_tick(.clk(sys_clk), .rst(sys_rst), .tick(led[2]));

heartBeatPulse #(.FREQ(FX3_FREQ)) 
fx3_tick(.clk(fx3_clk), .rst(fx3_rst), .tick(led[3]));
        
// new frame request	
logic [2:0]frame_req;  
logic frame_rst;

always @ (posedge sys_clk, posedge sys_rst) begin
       if (sys_rst) begin
          frame_req[2:0] <= '0;
          frame_rst <= 1'b0;
       end else begin                            
          frame_req[2:0] <= {frame_req[1:0], new_frame_req};
          frame_rst <= ~frame_req[2] & frame_req[1];
       end  
end

// Number of buffers to be uploaded to Fx3 ,for application layer engineer to futher develop
localparam BUF_NUM = 2;
// depth of each buffer to be uploaded to Fx3. The min depth is 7 bits and the max depth is 18 bits. for application layer engineer to futher develop
localparam logic [4:0] BUF_DEPTH [BUF_NUM-1:0] = {16,16};

// create data and data valid signals of the buffer to be uploaded to Fx3 @ sys_clk domain
logic [31:0]        resultData[BUF_NUM-1:0]; // databus is always 32 bits
logic [BUF_NUM-1:0] result_vld;
logic [BUF_NUM-1:0] result_ready; // signal or pulse to notify fx3 to grab this buff, for application layer engineer to futher develop

// pack buffer data, ready and valid
logic [BUF_NUM-1:0] buffDataVld;
logic [BUF_NUM-1:0] buffXerReady; 
logic [31:0] buffData [BUF_NUM-1:0]; 

always @ (posedge sys_clk) begin
   for (int k=0; k<BUF_NUM; k++) begin
       buffDataVld[k]  <= result_vld[k];
       buffXerReady[k] <= result_ready[k];
       buffData[k]     <= resultData[k];   
   end
end

// feedback pulse (sys_clk domain) to indicate the current buffer has been transferred (optional) 
logic [BUF_NUM-1:0] currentXferDone; // input to application layer

// debug image data 
logic [79:0] pixDataSync; // bulk out data from FX3 to application layer
logic  pixDatavldSync; //
logic bulkInStart;  // to notify application layer that fx3 is ready to accept bulk in data

// FX3 high speed parallel interface
logic  [7:0] fx3_dout;
assign fx3_db 	   = fx3_sloe_n ? fx3_dout : 8'hz; 
assign fx3_slcs_n  = 1'b0;

FX3SlaveFIFOInterface
#(
   .BUF_NUM   (BUF_NUM),
   .BUF_DEPTH (BUF_DEPTH)
)
FX3SlaveFIFOInterface_i 
(
         .fx3_clk   (fx3_clk),
		 .fx3_rst   (fx3_rst),
		 .sys_clk   (sys_clk),
		 .sys_rst   (sys_rst),		 
		 .frame_rst (frame_rst),		 
		 .fx3_din   (fx3_db),
		 .fx3_dout  (fx3_dout),
		 .fx3_flaga (fx3_flaga),
		 .fx3_a     (fx3_a),
		 .fx3_slrd_n(fx3_slrd_n),
		 .fx3_slwr_n(fx3_slwr_n), 
		 .fx3_sloe_n(fx3_sloe_n),	
		 .fx3_clk_s (fx3_clk_s),
		 .pixDataSync    (pixDataSync),
		 .pixDatavldSync (pixDatavldSync),
		 .imageRow       (2048),
		 .imageCol       (2448),
		 //buffer data and vld array.
		 .buffDataVld    (buffDataVld),
		 .buffData       (buffData),
		 .buffXerReady   (buffXerReady),
		 .currentXferDone(currentXferDone),
		 .bulkInStart    (bulkInStart),
		 .loopRead       ('0) // if enable it will loop the following buffer.  
);

// fill in the buffer that you want to bulk in to Fx3. 
logic [BUF_NUM-1:0] empty;
logic [BUF_NUM-1:0] wr_enable;
logic [31:0] wr_data[BUF_NUM-1:0];
genvar j;
generate
for (j=0; j<BUF_NUM; j=j+1)
begin 
sync_fifo
 #( .depth         ( 2**BUF_DEPTH[j] ) 
   ,.width         ( 32)
  )  
bulk_in_data
  ( .clk           ( sys_clk    )
  , .reset         ( sys_rst | frame_rst  )
  , .wr_enable     ( wr_enable[j]   ) // for application layer engineer to futher develop
  , .rd_enable     ( ~empty[j]      ) 
  , .wr_data       ( wr_data[j]     ) // for application layer engineer to futher develop
  , .rd_data       ( resultData[j]  ) 
  , .empty         ( empty[j]       ) 
  , .full          (                ) 
  , .count         (                ) 
  ) ; 
end
endgenerate

endmodule
