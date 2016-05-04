using Toybox.Communications as Comm;
using Toybox.WatchUi as Ui;
using Toybox.Graphics as Gfx;
using Toybox.Application as App;
using Toybox.System as Sys;

class BluetoothTransmitHelper {

	function transmitDataBatch(serializedData)
	{
		//var listener = new CommListener();
		var x = new CommListener();
		
		Sys.println("" + serializedData);
		
		Comm.transmit(serializedData, null, x);
	}	
}

class CommListener extends Comm.ConnectionListener
{
    function onComplete()
    {
        Sys.println( "Transmit Complete" );
    }

    function onError()
    {
        Sys.println( "Transmit Failed" );
    }
}