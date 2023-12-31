//*******************************************************************************************
//**
//**  File Name          : dp_ram.sv (SystemVerilog)
//**  Module Name        : dp_ram
//**                     :
//**  Module Description : This module will create an inferred dual port block ram
//**                     :
//**  Author             : Leon	
//**                     :
//**  Creation Date      : 1/2/2024
//**                     : 
//**  Version History    :
//**
//*******************************************************************************************

module dp_bram 
      #(parameter DATA_WIDTH = 8
       ,parameter ADDR_WIDTH = 6 )
       //-------------------------------------------------------------
       (input  logic                      clk_a
       ,input  logic                      clk_b
       
       ,input  logic                      we_a
       ,input  logic                      we_b
       
       ,input  logic  [(DATA_WIDTH-1):0]  data_a
       ,input  logic  [(DATA_WIDTH-1):0]  data_b
       
       ,input  logic  [(ADDR_WIDTH-1):0]  addr_a
       ,input  logic  [(ADDR_WIDTH-1):0]  addr_b
       
       ,output logic  [(DATA_WIDTH-1):0]  q_a
       ,output logic  [(DATA_WIDTH-1):0]  q_b
      );

    // Declare the RAM variable
    (* ram_style = "block" *) logic [DATA_WIDTH-1:0] ram[2**ADDR_WIDTH-1:0];

    always @ (posedge clk_a) begin // Port A 
      if (we_a) begin
        ram[addr_a] <= data_a;
        q_a         <= data_a;
      end else begin
        q_a <= ram[addr_a];
      end 
    end

    always @ (posedge clk_b) begin // Port B 
      if (we_b) begin
        ram[addr_b] <= data_b;
        q_b         <= data_b;
      end else begin
        q_b <= ram[addr_b];
      end 
    end

endmodule
