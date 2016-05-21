using Toybox.WatchUi as Ui;
using Toybox.Graphics as Gfx;
using Toybox.System as Sys;
using Toybox.Lang as Lang;

class SensorRetrieverAppView extends Ui.WatchFace {

	hidden var dataSentMessage;
	hidden var sensorTimer;
    hidden var hrData;
    hidden var acceleration;

	hidden var hrDataBuffer;
	hidden var accDataBuffer;
	hidden var mbHttpRequestsHelper;
	hidden var bleTransmitHelper;

    function initialize() {
        WatchFace.initialize();
        
        //JsonSerializationTests.testAccel();
        //JsonSerializationTests.testHr();
        
        // initialize accelerometer on a timer
        acceleration = new Acceleration(0, 0, 0);
        sensorTimer = new Timer.Timer();
        sensorTimer.start( method(:onTimerTick), 50, true );
        
        // initialize heart rate sensor
        Sensor.setEnabledSensors( [Sensor.SENSOR_HEARTRATE] );
        Sensor.enableSensorEvents( method( :onSensor) );
        
        // initialize others
		mbHttpRequestsHelper = new HttpMbRequestsHelper();
		bleTransmitHelper = new BluetoothTransmitHelper();
		accDataBuffer = new AccelerationBuffer();
		hrDataBuffer = new HrBuffer();
		
		//TESTE TESTE 
		mbHttpRequestsHelper.postAccData(JsonSerializationTests.testAccel());
    }
    
    function onTimerTick() {
    
    	var sensorInfo = Sensor.getInfo();
    	dataSentMessage = null;
    	
    	if (sensorInfo has :accel && sensorInfo.accel != null)
		{
			if (accDataBuffer.hasReachedBufferLimit())
			{
				//mbHttpRequestsHelper.postAccData(accDataBuffer);
				mbHttpRequestsHelper.postAccData(HttpMbRequestsHelper.bufferToJson(accDataBuffer));
				//bleTransmitHelper.transmitDataBatch(HttpMbRequestsHelper.bufferToJson(accDataBuffer));
				dataSentMessage = "Acc transmit ";
				accDataBuffer.resetBuffer();
			}
			else
			{
				acceleration = getAccelerationData(sensorInfo.accel);
				accDataBuffer.addSample(acceleration);
			}
		}
		else if (sensorInfo.accel == null)
		{
			System.println("Code got here");
			if (accDataBuffer.hasReachedBufferLimit())
			{
				mbHttpRequestsHelper.postAccData(HttpMbRequestsHelper.bufferToJson(accDataBuffer));
				//bleTransmitHelper.transmitDataBatch(HttpMbRequestsHelper.bufferToJson(accDataBuffer));
				dataSentMessage = "Acc transmit ";
				accelDataBuffer.resetBuffer();
			}
			else
			{
				acceleration = getDummyAccelerationData();
				accDataBuffer.addSample(acceleration);
			}
		}
    
    	Ui.requestUpdate();
    }
    
    function onSensor(sensorInfo){
    	
    	// get heart rate data
		var hr = sensorInfo.heartRate;
		if (hr == null)
		{
			hr = 0;
			
			return; // uncomment this for testing purpose only
		}
		
		dataSentMessage = null;

		if (hrDataBuffer.hasReachedBufferLimit())
    	{
    		//mbHttpRequestsHelper.postHrData(hrDataBuffer);
    		bleTransmitHelper.transmitDataBatch(HttpMbRequestsHelper.bufferToJson(hrDataBuffer));
    		//bleTransmitHelper.transmitDataBatch("{[\"Val\":\"0\",\"Val\":\"0\",\"Val\":\"0\",\"Val\":\"null\"]}");
    		
    		dataSentMessage = "Hr transmit ";
    		hrDataBuffer.resetBuffer();
    	}
    	else
    	{
	    	hrDataBuffer.addSample(hr);
	    	hrData = "Hr(" + hr  +")";
	    	System.println( hrData );
    	}
    	
    	// get acceleration data
		//var info = Sensor.getInfo();
		//var aaa = sensorInfo.altitude;
		
		//if (info has :accel && info.accel != null)
		//{
		//	acceleration = getAccelerationData(info.accel);
		//}
    	
    	Ui.requestUpdate();
    }
    
    function getAccelerationData(accel)
    {
    	var accX = accel[0];
    	var accY = accel[1] * -1; // cardinal y direction is opposite to the screen coordinates
		var accZ = accel[2];
    	return new Acceleration(accX, accY, accZ);
    }
    
    function getDummyAccelerationData()
    {
    	// MIZERIE pentru testare
    	var accX = 10.5;
    	var accY = 24.654;
		var accZ = 113.2;
    	return new Acceleration(accX, accY, accZ);
    }

    //! Load your resources here
    function onLayout(dc) {
        setLayout(Rez.Layouts.WatchFace(dc));
    }

    //! Called when this View is brought to the foreground. Restore
    //! the state of this View and prepare it to be shown. This includes
    //! loading resources into memory.
    function onShow() {
    }

    //! Update the view
    function onUpdate(dc) {
        // Get and show the current time

		var view = View.findDrawableById("TimeLabel");
		
		if (dataSentMessage == null)
		{
	        var clockTime = Sys.getClockTime();
	        var timeString = Lang.format("$1$:$2$", [clockTime.hour, clockTime.min.format("%02d")]);
	        
	        var accX = acceleration.getXAxisAcceleration();
	        var accY = acceleration.getYAxisAcceleration();
	        var accZ = acceleration.getZAxisAcceleration();
	        
	        if (hrData != null)
	        {
	        	timeString = timeString + " " + hrData + "\nX: " + accX + " Y: " + accY + " Z: " + accZ;	
	        }
	        else
	        {
	        	timeString = timeString + " " + "\nX: " + accX + " Y: " + accY + " Z: " + accZ;
	        }
	        
	        
	        view.setText(timeString);
        }
        else
        {
        	//dataSentMessage += bleTransmitHelper.getMessage();
        	view.setText(dataSentMessage);
        }

        // Call the parent onUpdate function to redraw the layout
        View.onUpdate(dc);
    }

    //! Called when this View is removed from the screen. Save the
    //! state of this View here. This includes freeing resources from
    //! memory.
    function onHide() {
    }

    //! The user has just looked at their watch. Timers and animations may be started here.
    function onExitSleep() {
    }

    //! Terminate any active timers and prepare for slow updates.
    function onEnterSleep() {
    }

}
