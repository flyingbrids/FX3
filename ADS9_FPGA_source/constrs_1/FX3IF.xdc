#FX3 slavefifo data bus
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[0]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[1]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[2]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[3]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[4]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[5]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[6]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_db[7]}]

set_property PACKAGE_PIN AH21 [get_ports {fx3_db[0]}]
set_property PACKAGE_PIN AJ21 [get_ports {fx3_db[1]}]
set_property PACKAGE_PIN AJ19 [get_ports {fx3_db[2]}]
set_property PACKAGE_PIN AJ20 [get_ports {fx3_db[3]}]
set_property PACKAGE_PIN AM12 [get_ports {fx3_db[4]}]
set_property PACKAGE_PIN AN12 [get_ports {fx3_db[5]}]
set_property PACKAGE_PIN AR13 [get_ports {fx3_db[6]}]
set_property PACKAGE_PIN AR12 [get_ports {fx3_db[7]}]

#FX3 slavefifo control bus
set_property IOSTANDARD LVCMOS18 [get_ports fx3_flaga]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_a[0]}]
set_property IOSTANDARD LVCMOS18 [get_ports {fx3_a[1]}]
set_property IOSTANDARD LVCMOS18 [get_ports fx3_slrd_n]
set_property IOSTANDARD LVCMOS18 [get_ports fx3_slwr_n]
set_property IOSTANDARD LVCMOS18 [get_ports fx3_slcs_n]
set_property IOSTANDARD LVCMOS18 [get_ports fx3_sloe_n]

set_property PACKAGE_PIN AW14 [get_ports fx3_flaga]
set_property PACKAGE_PIN AN13 [get_ports {fx3_a[0]}]
set_property PACKAGE_PIN AP13 [get_ports {fx3_a[1]}]
set_property PACKAGE_PIN AW13 [get_ports fx3_slrd_n]
set_property PACKAGE_PIN AV11 [get_ports fx3_slwr_n]
set_property PACKAGE_PIN AV16 [get_ports fx3_slcs_n]
set_property PACKAGE_PIN AU12 [get_ports fx3_sloe_n]

#FX3 GPIO
set_property IOSTANDARD LVCMOS18 [get_ports new_frame_req]
set_property PACKAGE_PIN AU10 [get_ports new_frame_req]


#slvfifo input delay
#specify the max/min databus propagation delay on FX3
#specify the flag propagation delay on FX3
#assume the trace delay on board
#assume the clock propagation delay from Fx3 to FPGA is the same as trace delay on board
#the bulk out clock is shifted by 2.5ns
#set input delay for databus
set_input_delay -clock fx3_bulkOut_clock -max 5.000 [get_ports {fx3_db[*]}]
set_input_delay -clock fx3_bulkOut_clock -min -1.000 [get_ports {fx3_db[*]}]
#set input delay for flag
set_input_delay -clock fx3_bulkOut_clock -max 6.000 [get_ports fx3_flaga]
set_input_delay -clock fx3_bulkOut_clock -min 5.000 [get_ports fx3_flaga]

#slvfifo output delay
#specify the setup/hold time of slrd_n
#set output delay for slrd_n
set_output_delay -clock fx3_bulkOut_clock -max 1.500 [get_ports fx3_slrd_n]
set_output_delay -clock fx3_bulkOut_clock -min -2.000 [get_ports fx3_slrd_n]

#specify the setup/hold time of sloe_n
#set output delay for sloe_n
set_output_delay -clock fx3_bulkIn_clock -max -5.000 [get_ports fx3_sloe_n]
set_output_delay -clock fx3_bulkIn_clock -min 7.000 [get_ports fx3_sloe_n]

#specify the setup/hold time of slwr_n
#set output delay for slwr_n
set_output_delay -clock fx3_bulkIn_clock -max 1.500 [get_ports fx3_slwr_n]
set_output_delay -clock fx3_bulkIn_clock -min 1.000 [get_ports fx3_slwr_n]

#specify the setup/hold time of addr
#set output delay for addr
set_output_delay -clock fx3_bulkOut_clock -max 1.500 [get_ports {fx3_a[*]}]
set_output_delay -clock fx3_bulkOut_clock -min -2.000 [get_ports {fx3_a[*]}]

#specify the setup/hold time of databus
#set output delay for databus
set_output_delay -clock fx3_bulkIn_clock -max 1.500 [get_ports {fx3_db[*]}]
set_output_delay -clock fx3_bulkIn_clock -min 1.000 [get_ports {fx3_db[*]}]

# set false path from output gate register to input data register of bi-direction IO
set_false_path -from [get_cells {FX3SlaveFIFOInterface_i/Fx3DataInterface_i/slwr_reg[0]}] -to [get_ports {fx3_db[*]}]





set_clock_groups -asynchronous -group [get_clocks [list [get_clocks -of_objects [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT1]] [get_clocks -of_objects [get_pins master_clk/inst/mmcme4_adv_inst/CLKOUT2]]]]
set_clock_groups -asynchronous -group [get_clocks [list [get_clocks -of_objects [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT1]] [get_clocks -of_objects [get_pins master_clk/inst/mmcme4_adv_inst/CLKOUT2]]]] -group [get_clocks [list [get_clocks -of_objects [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT0]] [get_clocks -of_objects [get_pins master_clk/inst/mmcme4_adv_inst/CLKOUT2]]]]
set_clock_groups -asynchronous -group [get_clocks [list [get_clocks -of_objects [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT0]] [get_clocks -of_objects [get_pins master_clk/inst/mmcme4_adv_inst/CLKOUT2]]]]
set_property C_CLK_INPUT_FREQ_HZ 300000000 [get_debug_cores dbg_hub]
set_property C_ENABLE_CLK_DIVIDER false [get_debug_cores dbg_hub]
set_property C_USER_SCAN_CHAIN 1 [get_debug_cores dbg_hub]
connect_debug_port dbg_hub/clk [get_nets sys_clk]
