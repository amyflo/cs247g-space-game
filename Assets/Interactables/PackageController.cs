﻿
using UnityEngine;
using System;

public class PackageController : Draggable {
    [SerializeField] private DialogueItem onPackageDeliveredText;
    [SerializeField] private DialogueItem onPackageDestroyedText;
    [SerializeField, Range(0f, 1f)] private float enterSpeed;
    [SerializeField] private AudioClip onPackageDestroyedSfx;
    [SerializeField] private AudioClip onPackageDeliveredSfx;
    private bool _pickedUp;
    [SerializeField] String flag; 

    private RegionController _currentRegion;
    private RegionController CurrentRegion {
        get => _currentRegion;
        set {
            // Current region is the same as last frame
            if (_currentRegion == value) {
                return;
            }
            
            // Current region has changed; exit previous region
            if (_currentRegion != null) {
                _currentRegion.OnRegionExit();
            }
            
            // Enter new region, if not null
            _currentRegion = value;
            if (_currentRegion != null) {
                _currentRegion.OnRegionEnter();
            }
        }
    }
    
    private new void OnMouseUp() {
        base.OnMouseUp();
        OnDrop(CurrentRegion);
        CurrentRegion = null;
    }
    
    private void OnDrop(RegionController region) {
        if (region != null) {
            if (region.RegionId == "deliver") {
                GlobalAudio.Source.PlayOneShot(onPackageDeliveredSfx);
                TextboxManager.Instance.CreateText(onPackageDeliveredText);
                if (flag == "decisionOne") {
                    PackageData.Instance.decisionOne = true;
                }
                Destroy(gameObject);
            } else if (region.RegionId == "destroy") {
                GlobalAudio.Source.PlayOneShot(onPackageDestroyedSfx);
                TextboxManager.Instance.CreateText(onPackageDestroyedText);
                if (flag == "decisionOne") {
                    PackageData.Instance.decisionOne = false;
                }
                Destroy(gameObject);
            }
        }
    }

    private new void Update() {
        base.Update();

        if (Holding) {
            _pickedUp = true;
            var mousePos = GetMousePosWorldCoordinates();
            var hit = Physics2D.Raycast(
                new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.z),
                Vector3.forward,
                100f,
                LayerMask.GetMask("Regions"));

            if (hit.collider != null) {
                CurrentRegion = hit.collider.GetComponent<RegionController>();
            } else {
                CurrentRegion = null;
            }
        }

        if (!_pickedUp) {
            rb.MovePosition(Vector2.Lerp(transform.position, Vector2.zero, enterSpeed));
        }
    }
}