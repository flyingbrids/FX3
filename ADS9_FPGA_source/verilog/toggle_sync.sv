//*******************************************************************************************
//**
//**  File Name          : toggle_sync.sv (SystemVerilog)
//**  Module Name        : toggle_sync
//**                     :
//**  Module Description : Toggle Synchronizer. A method to synchronize a 1 clock wide pulse
//**                     : from domain A to domain B. This is for multi-bit data valid crossing
//**                     : 
//**                     : 
//**                     :
//**  Author             : Leon Shen
//**                     :
//**  Creation Date      : 8/24/2015
//**                     : 
//**  Version History    :
//**
//*******************************************************************************************
module toggle_sync    (                      
                       input  logic  sig_in
                      ,input  logic  clk_b          // Clock B
                      ,input  logic  rst_b          // Reset B
                      ,output logic  sig_sync       // Signal out on domain B
                      ,output logic  pulse_sync     // pulse out on domain B
                      );

(*ASYNC_REG="TRUE"*) logic sig_b1;
(*ASYNC_REG="TRUE"*) logic sig_b2;                       
logic sig_b3 ;

always_ff @ (posedge clk_b or posedge rst_b) begin
  if (rst_b) begin
    sig_b1   <= '0 ;
    sig_b2   <= '0 ;
    sig_b3   <= '0 ;
    pulse_sync <= '0 ;
  end else begin
    sig_b1   <= sig_in  ;
    sig_b2   <= sig_b1  ;
    sig_b3   <= sig_b2  ;
    pulse_sync  <= ~sig_b3 & sig_b2 ; // rising edge
  end
end 

assign sig_sync = sig_b2;

endmodule