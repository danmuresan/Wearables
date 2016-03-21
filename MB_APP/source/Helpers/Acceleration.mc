class Acceleration {

	hidden var X;
	hidden var Y;
	hidden var Z;
	
	function initialize(x, y, z)
	{
	   //System.println( "X: " + X + "Y: " + Y + "Z: " + Z );
	   X = x;
	   Y = y;
	   Z = z;
	}

	function getXAxisAcceleration()
	{
		//System.println( "X: " + X );
		return X;
	}
	
	function getYAxisAcceleration()
	{
		//System.println( "Y: " + Y );
		return Y;
	}
	
	function getZAxisAcceleration()
	{
		//System.println( "Z: " + Z );
		return Z;
	}

}