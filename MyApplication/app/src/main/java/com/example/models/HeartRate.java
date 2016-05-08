package com.example.models;

/**
 * Created by danso_000 on 07.05.2016.
 */
public class HeartRate implements IDeviceDataModel {

    private int value;

    public HeartRate() {
    }

    public HeartRate(int val) {
        value = val;
    }

    public int getValue() {
        return value;
    }

    public void setValue(int value) {
        this.value = value;
    }
}
