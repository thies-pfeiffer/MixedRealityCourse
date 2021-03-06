//-----------------------------------------------------------------------
// <copyright file="AugmentedImageExampleController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.AugmentedImage
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controller for AugmentedImage example.
    /// </summary>
    public class AugmentedImageExampleController : MonoBehaviour
    {
        /// <summary>
        /// A prefab for visualizing an AugmentedImage.
        /// </summary>
        public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;

        /// <summary>
        /// The overlay containing the fit to scan user guide.
        /// </summary>
        public GameObject FitToScanOverlay;

        /// <summary>
        /// A container for showing AR content
        /// </summary>
        public Transform imagesContainer;


        private Dictionary<int, AugmentedImageVisualizer> m_Visualizers
            = new Dictionary<int, AugmentedImageVisualizer>();

        private Dictionary<int, Anchor> m_Anchors
            = new Dictionary<int, Anchor>();

        private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();


        /// <summary>
        /// The Unity Start method.
        /// </summary>
        public void Start()
        {
            // Disable all children
            for (int i = 0; i < imagesContainer.childCount; ++i)
            {
                imagesContainer.GetChild(i).gameObject.SetActive(false);
            }
        }



        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }


            // Check for touch interaction
            if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit raycastHit;
                if (Physics.Raycast(raycast, out raycastHit))
                {
                    Debug.Log("Hit " + raycastHit.collider.name);
                    raycastHit.collider.gameObject.SendMessage("ObjectTouched");
                }
            }

            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            // Get updated augmented images for this frame.
            Session.GetTrackables<AugmentedImage>(m_TempAugmentedImages, TrackableQueryFilter.Updated);

            // Create visualizers and anchors for updated augmented images that are tracking and do not previously
            // have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempAugmentedImages)
            {
                AugmentedImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
                if (image.TrackingState == TrackingState.Tracking)
                {
                    if (visualizer == null)
                    {
                        Debug.Log("Tracking " + image.DatabaseIndex);
                        // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                        Anchor anchor = image.CreateAnchor(image.CenterPose);
                        m_Anchors.Add(image.DatabaseIndex, anchor);
                        visualizer = (AugmentedImageVisualizer)Instantiate(AugmentedImageVisualizerPrefab, anchor.transform);
                        visualizer.Image = image;
                        m_Visualizers.Add(image.DatabaseIndex, visualizer);
                        // Get image container and make it visible
                        Transform imageContainer = imagesContainer.GetChild(image.DatabaseIndex);
                        if (imageContainer != null)
                        {
                            imageContainer.position = anchor.transform.position;
                            imageContainer.rotation = anchor.transform.rotation;
                            imageContainer.gameObject.SetActive(true);
                        }
                    } else
                    {
                        // If the image is continuing tracking, update the transformation
                        Transform imageContainer = imagesContainer.GetChild(image.DatabaseIndex);
                        if (imageContainer != null)
                        {
                            imageContainer.position = m_Anchors[image.DatabaseIndex].transform.position;
                            imageContainer.rotation = m_Anchors[image.DatabaseIndex].transform.rotation;
                        }
                    }
                }
                else if (image.TrackingState == TrackingState.Stopped && visualizer != null)
                {
                    m_Visualizers.Remove(image.DatabaseIndex);
                    m_Anchors.Remove(image.DatabaseIndex);
                    Transform imageContainer = imagesContainer.GetChild(image.DatabaseIndex);
                    if (imageContainer != null)
                    {
                        imageContainer.gameObject.SetActive(false);
                    }
                    GameObject.Destroy(visualizer.gameObject);
                }
            }

            // Show the fit-to-scan overlay if there are no images that are Tracking.
            foreach (var visualizer in m_Visualizers.Values)
            {
                if (visualizer.Image.TrackingState == TrackingState.Tracking)
                {
                    FitToScanOverlay.SetActive(false);
                    return;
                }
            }

            FitToScanOverlay.SetActive(true);



        }

    }
}
