using Toybox.Communications as Comm;
using Toybox.WatchUi as Ui;
using Toybox.Graphics as Gfx;
using Toybox.Application as App;
using Toybox.System as Sys;

class BluetoothTransmitHelper {

	hidden var commListener;

	function transmitDataBatch(serializedData)
	{
		//var listener = new CommListener();
		commListener = new CommListener();
		
		var dataAsString = serializedData + "";
		Sys.println(dataAsString);
		
		Comm.transmit(dataAsString, null, commListener);
	}
	
	/*
	function getMessage()
	{
		return commListener.getMessage();
	}*/
}

class CommListener extends Comm.ConnectionListener
{
    hidden var message = "";

	function getMessage()
	{
		return message;
	}

    function onComplete()
    {
        Sys.println( "Transmit Complete" );
        message = "complete";
    }

    function onError()
    {
        Sys.println( "Transmit Failed" );
        message = "failed";
    }
}