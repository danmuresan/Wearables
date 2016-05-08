package com.example.helpers;

import android.util.Log;

import com.example.models.Acceleration;
import com.example.models.IDeviceDataModel;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;

/**
 * Created by danso_000 on 07.05.2016.
 */
public class SerializationHelper {

    private final Gson m_gson = new Gson();

    private List<IDeviceDataModel> parseDeviceData(String objectAsString)
    {
        IDeviceDataModel[] objectsArray = m_gson.fromJson(objectAsString, IDeviceDataModel[].class);

        List<IDeviceDataModel> objects = Arrays.asList(objectsArray);

        return objects;
    }

    public ArrayList<Acceleration> getAccelerationDataBatch(String data)
    {
        List<IDeviceDataModel> genericData = parseDeviceData(data);
        ArrayList<Acceleration> accelerations = new ArrayList<>();

        for (IDeviceDataModel object : genericData)
        {
            try {
                accelerations.add((Acceleration)object);
            }
            catch (Exception ex)
            {
                Log.d("PARSE", ex.getMessage());
                return null;
            }
        }

        return accelerations;
    }

}
