using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;

namespace Bose.Wearable.Examples
{
    /// <summary>
    /// Main logic for the Advanced Demo. Begins a calibration routine on start, then spawns a new target on the surface
    /// of the icosphere. Spawns new targets after existing ones are collected.
    /// </summary>


    public class AdvancedDemoController : MonoBehaviour
    {
        /// <summary>
        /// A list of points corresponding to vertices on the icosphere. Does not include points that are difficult to
        /// look at.
        /// </summary>
            
        //Converting from Cartesian to Polar coordinates
        private static float x, y, z;
        private float r, theta, phi;

        private Vector3[] SPAWN_POINTS;
        private GameObject[] Ambient_Targets;
        private bool flag = true;
        public float Spatial_audio_delay = 15.0f;

        /// <summary>
        /// The prefab to spawn when creating a new target.
        /// For 6 targets, we would need 6 prefabs. Each will have their own sounds.
        /// </summary>
        [SerializeField]
        protected GameObject _targetPrefab, _targetPrefab1, _targetPrefab2, _targetPrefab3, _targetPrefab4, _targetPrefab5;

        /// <summary>
        /// The rotation matcher component attached to the rotation widget.
        /// </summary>
        [SerializeField]
        protected RotationMatcher _widgetRotationMatcher;

        /// <summary>
        /// The minimum time to wait before calibrating. Gives the user the opportunity to read and comprehend the
        /// calibration message.
        /// </summary>
        [SerializeField]
        protected float _minCalibrationTime;

        /// <summary>
        /// The maximum time to wait while calibrating.
        /// </summary>
        [SerializeField]
        protected float _maxCalibrationTime;

        /// <summary>
        /// The maximum allowable rotational velocity in degrees per second while calibrating. Waits for rotation to
        /// fall beneath this level before calibrating.
        /// </summary>
        [SerializeField]
        protected float _calibrationMotionThreshold;

        /// <summary>
        /// The amount of time to wait between target spawns in seconds.
        /// </summary>
        [SerializeField]
        protected float _spawnDelay;

        /// <summary>
        /// Invoked when calibration is complete.
        /// </summary>
        public event Action CalibrationCompleted;

        private WearableControl _wearableControl;

        private bool _calibrating;
        private float _calibrationStartTime;
        private int _lastSpawnPointIndex;
        private Quaternion _referenceRotation;



        private void Awake()
        {
            // Grab an instance of the WearableControl singleton. This is the primary access point to the wearable SDK.
            _wearableControl = WearableControl.Instance;
        }

        private void Start()
        {
            // Begin calibration immediately.
            StartCalibration();

            r = 1.0f;

            SPAWN_POINTS = new Vector3[6];
            int i = 0;
            // Conversion from cartesian to polar

            for(int phi = -90; phi <= 90; phi += 90)
            {
                if (phi == 0)
                {
                    for (int theta = 90; theta >= -180; theta -= 90)
                    {
                        x = r * Mathf.Cos(Mathf.Deg2Rad * phi) * Mathf.Cos(Mathf.Deg2Rad * theta);
                        y = r * Mathf.Sin(Mathf.Deg2Rad * phi);
                        z = r * Mathf.Cos(Mathf.Deg2Rad * phi) * Mathf.Sin(Mathf.Deg2Rad * theta);
                        Vector3 coordinate = new Vector3(x, y, z);
                        SPAWN_POINTS[i] = coordinate;
                        Debug.Log("Spawn point " + i + " " + SPAWN_POINTS[i]);
                        i++;
                       
                    }
                }
                else if(phi == 90)
                {
                    x = r * Mathf.Cos(Mathf.Deg2Rad * phi) * Mathf.Cos(Mathf.Deg2Rad * 0);
                    y = r * Mathf.Sin(Mathf.Deg2Rad * phi);
                    z = r * Mathf.Cos(Mathf.Deg2Rad * phi) * Mathf.Sin(Mathf.Deg2Rad * 0);
                    Vector3 coordinate = new Vector3(x, y, z);
                    SPAWN_POINTS[4] = coordinate;
                    Debug.Log("Spawn point " + "4" + " " + SPAWN_POINTS[4]);
                }
                else if(phi == -90)
                {
                    x = r * Mathf.Cos(Mathf.Deg2Rad * phi) * Mathf.Cos(Mathf.Deg2Rad * 0);
                    y = r * Mathf.Sin(Mathf.Deg2Rad * phi);
                    z = r * Mathf.Cos(Mathf.Deg2Rad * phi) * Mathf.Sin(Mathf.Deg2Rad * 0);
                    Vector3 coordinate = new Vector3(x, y, z);
                    SPAWN_POINTS[5] = coordinate;
                    Debug.Log("Spawn point " + "5" + " " + SPAWN_POINTS[5]);
                }
            }

            //Debug.Log(SPAWN_POINTS);
        }

        /// <summary>
        /// Begin the calibration routine. Waits for <see cref="_minCalibrationTime"/>, then until rotational
        /// velocity falls below <see cref="_calibrationMotionThreshold"/> before sampling the rotation sensor.
        /// Will not calibrate for longer than <see cref="_maxCalibrationTime"/>.
        /// </summary>
        private void StartCalibration()
        {
            _calibrating = true;
            _calibrationStartTime = Time.unscaledTime;
        }

        /// <summary>
        /// Spawns a new target on the surface of the icosphere.
        /// </summary>
        private void SpawnTarget()
        {

            // ORIGINAL CODE

            // // Randomly select a new spawn point not equal to the previous point.
            // _lastSpawnPointIndex = (Random.Range(1, SPAWN_POINTS.Length / 2) + _lastSpawnPointIndex) % SPAWN_POINTS.Length;

            // // Create a new target object at that point, parented to the controller.
            // GameObject target = Instantiate(_targetPrefab, transform);
            // target.transform.position = SPAWN_POINTS[_lastSpawnPointIndex] * 0.5f;

            // // Subscribe to the new target's collection event.
            // TargetController targetController = target.GetComponent<TargetController>();
            // targetController.Collected += OnCollected;

            // // Pass on the reference rotation from calibration to the target controller.
            // targetController.ReferenceRotation = _referenceRotation;



            // MODIFIED CODE BY AKSHANSH
            // Idea: Remove the random selector and instantiate 3 objects at once. Each one will have its own target controller

            // TARGET 1 INSTANTIATED
            // Create a new target object at that point, parented to the controller.
            GameObject target1 = Instantiate(_targetPrefab, transform);
            target1.transform.position = SPAWN_POINTS[0] * 0.5f;

            Debug.Log("Target 1 instantiated at location: " + target1.transform.position);

            // Subscribe to the new target's collection event.
            TargetController targetController1 = target1.GetComponent<TargetController>();
            targetController1.Collected += OnCollected;

            // Pass on the reference rotation from calibration to the target controller.
            targetController1.ReferenceRotation = _referenceRotation;

            // TARGET 2 INSTANTIATED
            // Create a new target object at that point, parented to the controller.
            GameObject target2 = Instantiate(_targetPrefab1, transform);
            target2.transform.position = SPAWN_POINTS[1] * 0.5f;

            Debug.Log("Target 2 instantiated at location: " + target2.transform.position);

            // Subscribe to the new target's collection event.
            TargetController targetController2 = target2.GetComponent<TargetController>();
            targetController2.Collected += OnCollected;

            // Pass on the reference rotation from calibration to the target controller.
            targetController2.ReferenceRotation = _referenceRotation;

            // TARGET 3 INSTANTIATED
            // Create a new target object at that point, parented to the controller.
            GameObject target3 = Instantiate(_targetPrefab2, transform);
            target3.transform.position = SPAWN_POINTS[2] * 0.5f;

            Debug.Log("Target 3 instantiated at location: " + target3.transform.position);

            // Subscribe to the new target's collection event.
            TargetController targetController3 = target3.GetComponent<TargetController>();
            targetController3.Collected += OnCollected;

            // Pass on the reference rotation from calibration to the target controller.
            targetController3.ReferenceRotation = _referenceRotation;


            // TARGET 4 INSTANTIATED
            // Create a new target object at that point, parented to the controller.
            GameObject target4 = Instantiate(_targetPrefab3, transform);
            target4.transform.position = SPAWN_POINTS[3] * 0.5f;

            Debug.Log("Target 4 instantiated at location: " + target4.transform.position);

            // Subscribe to the new target's collection event.
            TargetController targetController4 = target4.GetComponent<TargetController>();
            targetController4.Collected += OnCollected;

            // Pass on the reference rotation from calibration to the target controller.
            targetController4.ReferenceRotation = _referenceRotation;


            // TARGET 5 INSTANTIATED
            // Create a new target object at that point, parented to the controller.
            GameObject target5 = Instantiate(_targetPrefab4, transform);
            target5.transform.position = SPAWN_POINTS[4] * 0.5f;
            
            Debug.Log("Target 5 instantiated at location: " + target5.transform.position);

            // Subscribe to the new target's collection event.
            TargetController targetController5 = target5.GetComponent<TargetController>();
            targetController5.Collected += OnCollected;

            // Pass on the reference rotation from calibration to the target controller.
            targetController5.ReferenceRotation = _referenceRotation;


            // TARGET 6 INSTANTIATED
            // Create a new target object at that point, parented to the controller.
            GameObject target6 = Instantiate(_targetPrefab5, transform);
            target6.transform.position = SPAWN_POINTS[5] * 0.5f;

            Debug.Log("Target 6 instantiated at location: " + target6.transform.position);

            // Subscribe to the new target's collection event.
            TargetController targetController6 = target5.GetComponent<TargetController>();
            targetController6.Collected += OnCollected;

            // Pass on the reference rotation from calibration to the target controller.
            targetController6.ReferenceRotation = _referenceRotation;

            Ambient_Targets = new GameObject[] { target2, target3, target4, target5, target6 };

            for (int i = 0; i < Ambient_Targets.Length; i++)
            {
                Transform[] ts = Ambient_Targets[i].transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in ts)
                {
                    if (t.gameObject.name == "SFX Loop Close" || t.gameObject.name == "SFX Loop Middle" || t.gameObject.name == "SFX Loop Far")
                    {
                        t.gameObject.GetComponent<AudioSource>().mute = true;
                        Debug.Log("Child " + i + " is " + t.gameObject.name + " " + t.gameObject.activeSelf);
                    }
                }
            }

        }

        /// <summary>
        /// Invoked when a target is collected.
        /// </summary>
        /// <param name="target">The target that was collected</param>
        private void OnCollected(TargetController target)
        {
            // Spawn a new target after a delay.
            //Invoke("SpawnTarget", _spawnDelay);
        }

        private void Update()
        {
            if (flag)
            {
                StartCoroutine(WaitforIntro(Spatial_audio_delay));
                flag = false;
            }

            if (_calibrating)
            {
                // While calibrating, continuously sample the gyroscope and wait for it to fall below a motion
                // threshold. When that happens, or a timeout is exceeded, grab a sample from the rotation sensor and
                //  use that as the reference rotation.
                SensorFrame frame = _wearableControl.LastSensorFrame;

                bool didWaitEnough = Time.unscaledTime > _calibrationStartTime + _minCalibrationTime;
                bool isStationary = frame.angularVelocity.value.magnitude < _calibrationMotionThreshold;
                bool didTimeout = Time.unscaledTime > _calibrationStartTime + _maxCalibrationTime;

                if ((didWaitEnough && isStationary) || didTimeout)
                {
                    _referenceRotation = frame.rotationNineDof;
                    _calibrating = false;

                    // Pass along the reference to the rotation matcher on the widget.
                    _widgetRotationMatcher.SetRelativeReference(frame.rotationNineDof);

                    if (CalibrationCompleted != null)
                    {
                        CalibrationCompleted.Invoke();
                    }

                    // Spawn the first target after calibration completes.
                    Invoke("SpawnTarget", _spawnDelay);
                }
            }
        }

        IEnumerator WaitforIntro(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            for (int i = 0; i < Ambient_Targets.Length; i++)
            {
                Transform[] ts = Ambient_Targets[i].transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in ts)
                {
                    if (t.gameObject.name == "SFX Loop Close" || t.gameObject.name == "SFX Loop Middle" || t.gameObject.name == "SFX Loop Far")
                    {
                        /*
                        AudioClip clip = Resources.Load<AudioClip>(Audio_paths[i]);
                        t.gameObject.GetComponent<AudioSource>().clip = clip;
                        t.gameObject.GetComponent<AudioSource>().Play();
                        */
                        t.gameObject.GetComponent<AudioSource>().mute = false;
                        Debug.Log("Deactivated Child " + i + " is " + t.gameObject.name + " " + t.gameObject.activeSelf);
                    }
                }
            }
        }
    }
}
