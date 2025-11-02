//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {

    // Save rotation here
    Vector3 rotation = Vector3.zero;
    
    // Set speed, 0.25 gives us a 24 minute day, using ~0.0104167 will give us a 24 hour day!
    public float speed = 6f;

    // By simply lerping between these values we can get the correct fog
    public Color dayFog;
    public Color nightFog;

    [HideInInspector]
    public float time;
    public float day;
    public float lightLevel;

    float rotationTime;

    private void Update() {

        CalculateTime();
        CalculateLightLevel();

        RenderSettings.fogColor = Color.Lerp(nightFog, dayFog, lightLevel);

    }

    private void CalculateTime() {

        // Rotates the sun based off a 24 minute day!
        rotation.x = speed * Time.deltaTime;
        transform.Rotate(rotation, Space.World);

        // This does all the time/ day calculation stuff!
        // We can get the total rotation and then
        // divide it by 360 and multiply it by 24 to get the hour count
        // we can also floor to int it to get te day count
        // and if we take our total time and subtract it by day * 24 we get the current time of day!
        rotationTime += speed * Time.deltaTime;
        day = Mathf.FloorToInt(rotationTime / 360f);
        time = ((rotationTime / 360f) * 24f) - (day * 24f);

    }

    // Without the internet this would've been impossible!
    private void CalculateLightLevel() {

        // ~~~Time Refrences!~~~ //
        // 22 - 6: Morning sunrise
        // 6 - 14: Afternoon sunset
        // 14 - 22: Complete darkness
        // All in intervals of 8

        // Morning sunrise 
        if (time >= 22f || time < 6f) {
            float intervalTime = (time >= 22f) ? time - 22f : (time + 24f) - 22f;
            lightLevel = Mathf.Clamp01(intervalTime / 8f);
            return;
        }

        // Afternoon sunset
        if (time >= 6f && time < 14f) {
            float intervalTime = time - 6f;
            lightLevel = Mathf.Clamp01(1f - (intervalTime / 8f));
            return;
        }

        // Complete darkness
        lightLevel = 0f;

    }

}
