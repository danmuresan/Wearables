class HrBuffer extends SensorDataBuffer {

	const HR_BUFFER_SIZE = 5;
	
	hidden var hrBuffer;
	hidden var hrBufferIndex;
	hidden var hrBufferSize;
	
	function initialize(size) {
		
		if (size == null)
		{
			hrBufferSize = HR_BUFFER_SIZE;
		}
		else
		{
			hrBufferSize = size;
		}
		
		hrBuffer = new [hrBufferSize];
		hrBufferIndex = 0;
	}
	
 	function addSample( hr ) {
 		if (hrBufferIndex < hrBufferSize)
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
		return hrBufferSize;
	}
	
	function hasReachedBufferLimit() {
		return hrBufferIndex == hrBufferSize - 1;
	}
	
	function getElementAt(index){
		return hrBuffer[index];
	}
	
	function resetBuffer() {
		hrBufferIndex = 0;
	}
	
}