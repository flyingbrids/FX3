//**
//**  File Name          : reset_sync.sv (SystemVerilog)
//**  Module Name        : reset_sync
//**                     :
//**  Module Description : This module takes in a top level or board level reset and creates
//**                     : the global reset tree. Asynchronous reset assert - Synchronous 
//**                     : reset de-assert. This is intended to be used with a top level
//**                     : PLL.
//**                     :
//**  Author             : Leon Shen
//**                     :
//**  Creation Date      : 1/27/2015
//**                     : 
//**  Version History    :
//**
//*******************************************************************************************

module reset_sync        (input  logic  pll_clk          // Clock source from PLL
                         ,input  logic  pll_lock         // PLL Lock signal
                         ,input  logic  external_rst     // External reset
                         ,output logic  sync_rst_out     // Global Reset
                         ,output logic  sync_rst_out_n   // Global (optional) Reset out
                        );

(* ASYNC_REG = "TRUE" *) logic meta_rst_out ;
logic async_rst;
  
assign async_rst = external_rst | ~pll_lock ; // if lock is lost or external reset asserted
                        
always_ff @ (posedge pll_clk or posedge async_rst) begin
  if (async_rst) begin
    sync_rst_out   <= 1'b1;
    meta_rst_out   <= 1'b1;
    sync_rst_out_n <= 1'b0;
  end else begin
    meta_rst_out   <= 1'b0;
    sync_rst_out   <= meta_rst_out ;
    sync_rst_out_n <= ~meta_rst_out ;
  end
end 


endmodule
