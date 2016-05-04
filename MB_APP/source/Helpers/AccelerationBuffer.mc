class AccelerationBuffer extends SensorDataBuffer {
	
	const ACCEL_BUFFER_SIZE = 100;
	
	hidden var accelBufferIndex;
	hidden var accelBuffer;
	hidden var accelBufferSize;
	
	function initialize(size) {
	
		if (size == null)
		{
			accelBufferSize = ACCEL_BUFFER_SIZE;	
		}
		else
		{
			accelBufferSize = size;
		}
		
		accelBuffer = new [accelBufferSize];
		accelBufferIndex = 0;
	}
	
	function addSample( acc ) {
		if (accelBufferIndex < accelBufferSize)
		{
			accelBuffer[accelBufferIndex] = acc;
			accelBufferIndex++;
		}
		else
		{
			System.println( "Reached acc buffer limit, reseting index." );
			accelBufferIndex = 0;
			accelBuffer[accelBufferIndex] = acc;
			accelBufferIndex++;
		}
	}
	 
 	function getBufferLength() {
		return accelBufferSize;
	}
	
	function hasReachedBufferLimit() {
		return accelBufferIndex == accelBufferSize - 1; 
	}
	
	function getElementAt(index){
		return accelBuffer[index];
	}
	
	function resetBuffer() {
		accelBufferIndex = 0;
	}
	
}