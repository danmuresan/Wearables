class HrBuffer extends SensorDataBuffer {

	const HR_BUFFER_SIZE = 10;
	
	hidden var hrBuffer;
	hidden var hrBufferIndex;
	
	function initialize() {
		hrBuffer = new [HR_BUFFER_SIZE];
		hrBufferIndex = 0;
	}
	
 	function addSample( hr ) {
 		if (hrBufferIndex < HR_BUFFER_SIZE)
 		{
 			hrBuffer[hrBufferIndex] = hr;
 			hrBufferIndex++;
 		}
 		else
 		{
 			System.println( "Reached hr buffer limit, reseting index." );
 			hrBufferIndex = 0;
 			hrBuffer[hrBufferIndex] = hr;
			hrBufferIndex++;
 		}
 	}

	function getBufferLength() {
		return hrBufferIndex;
	}
	
	function hasReachedBufferLimit() {
		return hrBufferIndex == HR_BUFFER_SIZE - 1;
	}

}