# AD9576 OUT6
set_property IOSTANDARD DIFF_HSTL_I_18 [get_ports AD9576_OUT6_n]
set_property IOSTANDARD DIFF_HSTL_I_18 [get_ports AD9576_OUT6_p]
set_property PACKAGE_PIN AN21 [get_ports AD9576_OUT6_p]
set_property PACKAGE_PIN AN20 [get_ports AD9576_OUT6_n]

#SMA output
set_property IOSTANDARD LVCMOS18 [get_ports baseClkforAD9528]
set_property PACKAGE_PIN G22 [get_ports baseClkforAD9528]

#FX3 clk output
set_property IOSTANDARD LVCMOS18 [get_ports fx3_pclk]
set_property PACKAGE_PIN AN18 [get_ports fx3_pclk]

#LED output
set_property IOSTANDARD LVCMOS18 [get_ports {led[0]}]
set_property IOSTANDARD LVCMOS18 [get_ports {led[1]}]
set_property IOSTANDARD LVCMOS18 [get_ports {led[2]}]
set_property IOSTANDARD LVCMOS18 [get_ports {led[3]}]
set_property IOSTANDARD LVCMOS18 [get_ports {led[4]}]
set_property IOSTANDARD LVCMOS18 [get_ports {led[5]}]

set_property PACKAGE_PIN B21 [get_ports {led[0]}]
set_property PACKAGE_PIN A22 [get_ports {led[1]}]
set_property PACKAGE_PIN B22 [get_ports {led[2]}]
set_property PACKAGE_PIN B25 [get_ports {led[3]}]
set_property PACKAGE_PIN B24 [get_ports {led[4]}]
set_property PACKAGE_PIN C24 [get_ports {led[5]}]


#create clock and clock group
create_generated_clock -name system_clock_master -source [get_pins master_clk/inst/mmcme4_adv_inst/CLKIN1] -master_clock [get_clocks AD9576_OUT6_p] [get_pins master_clk/inst/mmcme4_adv_inst/CLKOUT2]
create_generated_clock -name fx3_bulkOut_clock -source [get_pins fx3_PLL/inst/plle4_adv_inst/CLKIN] -master_clock [get_clocks fx3_pclk] [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT0]
create_generated_clock -name fx3_bulkIn_clock -source [get_pins fx3_PLL/inst/plle4_adv_inst/CLKIN] -master_clock [get_clocks fx3_pclk] [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT1]

set_clock_groups -physically_exclusive -group [get_clocks -of_objects [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT1]] -group [get_clocks -of_objects [get_pins fx3_PLL/inst/plle4_adv_inst/CLKOUT0]]

#false path
set_false_path -from * -to [get_ports *led*]

#debug core

