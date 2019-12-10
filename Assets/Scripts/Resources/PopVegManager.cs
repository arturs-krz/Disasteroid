using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class PopVegManager : MonoBehaviour
{
    //Variabels for updating UI
    public Slider popSlider;
    
    // Within that radius, there are some injuries.
    public int effectRadius;

    // Within that radius, everyone dies.
    public int exterminationRadius;

    // pop_table is a tab with dimensions 180 x 360
    // pop_table[i][j] corresponds to the population density of the lattitude i-89.5 and longitude j-179.5
    public static float[][] pop_table;
    public static long totalPop;
    public static float[][] veg_table;
    public static float totalVeg;

    // All necessary CO2 information
    public CO2Manager CO2Manage;
    public float CO2CurrentValue;
    public float CO2Ratio;
    public float CO2CriticalValue;

    // Rate at which population automatically increases over time
    public float popIncreaseRate;

    // 
    
    // Start is called before the first frame update
    void Start()
    {
        using (var reader = new StreamReader(@"Assets/Data/population_data.csv"))
        {
            pop_table = new float[180][];
            
            var line = reader.ReadLine();
            int k = 0;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                var values = line.Split(',');
                pop_table[179 - k] = new float[360];
                for (int i = 1; i < 361; i++) {
                    if (values[i] == "99999.0")
                    {
                        pop_table[179-k][i - 1] = 0;
                    }
                    else {
                        pop_table[179-k][i - 1] = float.Parse(values[i].Replace(".",","));
                    }
                    
                }
                k += 1;
            }
        }
        using (var reader = new StreamReader(@"Assets/Data/vegetation_data.csv"))
        {
            veg_table = new float[180][];

            var line = reader.ReadLine();
            int k = 0;
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                var values = line.Split(',');
                veg_table[179 - k] = new float[360];
                for (int i = 1; i < 361; i++)
                {
                    if (values[i] == "99999.0")
                    {
                        veg_table[179 - k][i - 1] = 0;
                    }
                    else
                    {
                        veg_table[179 - k][i - 1] = float.Parse(values[i].Replace(".", ","))+0.1f;
                    }
                }
                k += 1;
            }
        }
        totalVeg = TotalVegetation();
        totalPop = TotalPopulation();

        // Set population UI
        popSlider.maxValue = totalPop;
        popSlider.value = totalPop;

        popIncreaseRate = 1;

        // Set CO2 variables
        CO2Manage = GetComponent<CO2Manager>();
        CO2CurrentValue = CO2Manage.currentCO2;
        CO2CriticalValue = CO2Manage.criticalCO2;

        Debug.Log("At start CO2CurrentValue is: " + CO2CurrentValue);
        Debug.Log("At start CO2CriticalValue is: " + CO2CriticalValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            // Execute block of code here

            // Have population increase automatically over time, rate dependent on CO2 levels
            CO2Ratio = (CO2CriticalValue - CO2CurrentValue) / CO2CriticalValue;

            NetworkDebugger.Log("In IncreasePopUdateUI the CO2CurrentValue is: " + CO2CurrentValue);
            NetworkDebugger.Log("In IncreasePopUdateUI the CO2CriticalValue is: " + CO2CriticalValue);
            NetworkDebugger.Log("In IncreasePopUdateUI the CO2Ratio is: " + CO2Ratio);

            totalPop += Convert.ToInt64(popIncreaseRate * CO2Ratio * Time.deltaTime);
            NetworkDebugger.Log("totalPop is: " + totalPop);

            // Update population UI
            popSlider.value = totalPop;
        }
        //InvokeRepeating("IncreasePopUpdateUI", 0f, 0.5f);
    }

    public long TotalPopulation() {
        long pop = 0;
        for (int i = 0; i < 180; i++)
        {
            for (int j = 0; j < 360; j++)
            {
                pop += (int)(pop_table[i][j] * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
            }
        }
        return pop;
    }

    public long TotalVegetation()
    {
        long veg = 0;
        for (int i = 0; i < 180; i++)
        {
            for (int j = 0; j < 360; j++)
            {
                veg += (int)(veg_table[i][j] * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
            }
        }
        return veg;
    }

    public long Explosion(float latitude, float longitude, out long number_of_dead, out float dead_vegetation)
    {
        number_of_dead = 0;
        dead_vegetation = 0;
        for (int i = 0; i < 180; i++) {
            for (int j = 0; j < 360; j++) {
                var x = (latitude - (i - 89.5f)) * Math.Cos((longitude + ((float)j - 89.5f))*Math.PI/180 / 2);
                var y = (longitude - (j - 179.5f));
                var explosionDistance = Math.Sqrt(x * x + y * y) ;
                if (explosionDistance < exterminationRadius) {
                    float density_dead = pop_table[i][j];
                    pop_table[i][j] = 0;
                    number_of_dead += (long) (density_dead* 12345 * Math.Cos(((double)i - 89.5)*Math.PI/ 180d));
                    dead_vegetation += (float) (veg_table[i][j] * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
                    veg_table[i][j] = 0;
                }
                else if(explosionDistance < effectRadius){
                    float proportion = (effectRadius - (float) explosionDistance) / (effectRadius - exterminationRadius);
                    float density_dead = proportion*pop_table[i][j];
                    number_of_dead += (long)(density_dead * 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
                    pop_table[i][j] *= (1 - proportion);
                    dead_vegetation += (float)(veg_table[i][j]*proportion* 12345 * Math.Cos(((double)i - 89.5) * Math.PI / 180d));
                    veg_table[i][j] *= (1 - proportion);
                }
            }
        }
        totalPop -= number_of_dead;
        totalVeg -= dead_vegetation;
        return totalPop;
    }
    public long Explosion(Vector2 v, out long number_of_dead, out float dead_vegetation) {
        return Explosion(v.x, v.y, out number_of_dead, out dead_vegetation);
    }
}
