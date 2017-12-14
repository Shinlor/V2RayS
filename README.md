# V2RayS
一个V2Ray的Windows客户端

## 放入V2Ray文件夹使用

## PAC文件
本程序自带了一个PAC服务，使用PAC文件而不是V2Ray自带的路由模式，会自动修改IE的代理设置（目前暂不允许自定义代理端口），可在通知栏图标切换全局代理模式和PAC模式，因此在V2Ray目录下需要有一个PAC.txt文件，可以使用本程序自带的PAC文件，或者自定义的有效的任何PAC文件，但文件名需为PAC.txt

因为V2Ray本身支持http传入，因此本地代理可以直接为http代理，不需要使用privoxy，考虑再三，本程序暂不提供socks/http代理的选择，本来考虑强制修改模式为http，后来觉得不合适，因此注意V2Ray配置文件里面inbound的"protocol"协议最好为http

## config.json文件
本程序存储的服务器设置只会修改VMess协议下的服务设置，暂不支持SS协议下的设置，其他设置如mKCP等不会修改，因此最好确认在V2Ray下的config.json文件有效可用。V2Ray自带的config.json文件可以使用，只需要在本程序中修改服务器设置即可。修改了服务器设置保存后，记得选择“应用到V2Ray”生效

## 本程序运行需要.NET Framework 4.6.1