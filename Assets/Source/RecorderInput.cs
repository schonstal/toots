﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class RecorderInput : MonoBehaviour {
  public static int SAMPLES = 8192;
  public static int SAMPLE_RATE = 44100;

  public float tolerance = 100f;

  float highNote = 0f;
  float lowNote = 0f;
  float maxFrequency = 0.0f;

  float highThreshold = 0.001f;
  float lowThreshold = 0.001f;

  public bool IsInitialized {
    get {
      return initialized;
    }
  }
  
  public bool IsCalibrated {
    get {
      return highNote > 0 && lowNote > 0;
    }
  }

  public bool HasHighNote {
    get {
      return highNote > 0;
    }
  }

  bool initialized = false;
  string deviceName = "Princess";

  float[] spectrum = new float[SAMPLES];
  AudioSource audioSource;
  float frequency = 0.0f;

  void Start() {
  }

  void Update() {
    if (Input.GetKeyDown("r")) {
      highNote = 0;
      lowNote = 0;
      highThreshold = 0;
      lowThreshold = 0;
      initialized = false;
      Microphone.End(deviceName);
    }
    if (!initialized) return;

    frequency = GetFrequency();

    if (highNote <= 0) {
      if (Input.GetKeyDown("space")) {
        highNote = frequency;
        highThreshold = maxFrequency * 0.1f;
      }
      return;
    } else if (lowNote <= 0) {
      if (Input.GetKeyDown("space")) {
        lowNote = frequency;
        lowThreshold = maxFrequency * 0.1f;
      }
      return;
    }
  }

  float GetFrequency() {
    float frequency = 0.0f;
    audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    int index = 0;
    maxFrequency = 0.0f;
    for (int i = 1; i < SAMPLES; i++) {
      if (maxFrequency < spectrum[i-1]) {
        maxFrequency = spectrum[i-1];
        index = i;
      }
    }
    frequency = index * SAMPLE_RATE / SAMPLES;
    return frequency;
  }

  bool noteTriggered(float note) {
    if(!IsInitialized) return false;
    return frequency > note - tolerance && frequency < note + tolerance;
  }

  public bool lowTriggered() {
    return noteTriggered(highNote) && (maxFrequency > lowThreshold);
  }

  public bool highTriggered() {
    return noteTriggered(lowNote) && (maxFrequency > highThreshold);
  }

  public void initialize(string device) {
    deviceName = device;
    var iteration = 0;
    if(!Microphone.devices.Contains(deviceName)) return;
    audioSource = GetComponent<AudioSource>();
    audioSource.clip = Microphone.Start(deviceName, true, 1, SAMPLE_RATE);
    audioSource.loop = true;
    while(!(Microphone.GetPosition(deviceName) > 0)) { iteration++; if(iteration > 10000) break;}
    audioSource.Play();
    initialized = true;
  }
}
