﻿using UnityEngine;
using System.Collections;

// sets the focal point for the camera
public class PlayerFocalPoint : MonoBehaviour
{
    #region vars
    private CharacterStats char_stats;
    private MagGripUpgrade mag_grip;
    private const int DEFAULT_X = 40;
    private const int DEFAULT_Y = 20;
    private const float FLIP_DURATION = 0.4f; // seconds
    private const int depth = -10; // camera z depth

    private Vector3 center_position;
    private Vector3 right_position;
    private Vector3 left_position;
    private Vector3 below_position;
    private Vector2 focal_point_slider;
    private bool is_following_player;
    #endregion

    // Use this for early initialization (references)
    private void Awake()
    {
        char_stats = GetComponentInParent<CharacterStats>();
        mag_grip   = GetComponentInParent<MagGripUpgrade>();
    }

    // Use this for initialization
    void Start()
    {
        right_position  = new Vector3(  DEFAULT_X, DEFAULT_Y, depth );
        left_position   = new Vector3( -DEFAULT_X, DEFAULT_Y, depth );
        center_position = new Vector3( 0.0f, DEFAULT_Y, depth );
        below_position  = new Vector3( 0.0f, DEFAULT_Y - 40.0f, depth);
        focal_point_slider = new Vector3( 1.0f, 1.0f );
        is_following_player = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ( is_following_player == true )
        {
            // X
            float increment = 0.0f;
            if ( ( char_stats.current_master_state == CharEnums.MasterState.DefaultState && char_stats.IsFacingRight() ) ||
                 ( char_stats.current_master_state == CharEnums.MasterState.ClimbState 
                   && mag_grip.current_climb_state == MagGripUpgrade.ClimbState.WallClimb
                   && char_stats.IsFacingLeft() && mag_grip.is_looking_away ) )
            {
                increment = Time.deltaTime * Time.timeScale / FLIP_DURATION;
            }
            else if ( ( char_stats.current_master_state == CharEnums.MasterState.DefaultState && char_stats.IsFacingLeft() ) ||
                      ( char_stats.current_master_state == CharEnums.MasterState.ClimbState 
                        && mag_grip.current_climb_state == MagGripUpgrade.ClimbState.WallClimb 
                        && char_stats.IsFacingRight() && mag_grip.is_looking_away ) )
            {
                increment = -1.0f * Time.deltaTime * Time.timeScale / FLIP_DURATION;
            }
            else if ( ( char_stats.current_master_state == CharEnums.MasterState.ClimbState
                        && mag_grip.current_climb_state == MagGripUpgrade.ClimbState.WallClimb
                        && char_stats.IsFacingLeft() && ! mag_grip.is_looking_away ) ||
                      ( char_stats.current_master_state == CharEnums.MasterState.ClimbState 
                        && mag_grip.current_climb_state == MagGripUpgrade.ClimbState.WallClimb 
                        && char_stats.IsFacingRight() && ! mag_grip.is_looking_away ) ||
                      ( char_stats.current_master_state == CharEnums.MasterState.ClimbState
                        && mag_grip.current_climb_state == MagGripUpgrade.ClimbState.CeilingClimb ) )
            {
                // aim for center ( 0.5 value )
                float dist_to_center = 0.5f - focal_point_slider.x;
                increment = Time.deltaTime * Time.timeScale / FLIP_DURATION;
                if ( dist_to_center < 0.0f ) { increment = -1.0f * increment; }
                if ( Mathf.Abs( dist_to_center ) < Mathf.Abs( increment ) ) { increment = dist_to_center; }
            }
            focal_point_slider.x = Mathf.Clamp( focal_point_slider.x + increment, 0.0f, 1.0f );

            // Y
            if ( char_stats.current_master_state == CharEnums.MasterState.ClimbState
                 && mag_grip.current_climb_state == MagGripUpgrade.ClimbState.CeilingClimb
                 && mag_grip.is_looking_away )
            {
                increment = -1.0f * Time.deltaTime * Time.timeScale / FLIP_DURATION;
            }
            else
            {
                increment = Time.deltaTime * Time.timeScale / FLIP_DURATION;
            }
            focal_point_slider.y = Mathf.Clamp( focal_point_slider.y + increment, 0.0f, 1.0f );
            
            float x = Mathf.Lerp( left_position.x,  right_position.x,  focal_point_slider.x );
            float y = Mathf.Lerp( below_position.y, center_position.y, focal_point_slider.y );
            transform.localPosition = new Vector3( x, y, depth );
        }
    }

    /// <summary>
    /// Stops the focal point from following the player
    /// </summary>
    void StopFollowingPlayer()
    {
        is_following_player = false;
    }

    /// <summary>
    /// Sets the focal point to follow the player
    /// </summary>
    void FollowPlayer()
    {
        is_following_player = true;
    }

    /// <summary>
    /// Moves the focal point to a specific location relative to the player
    /// </summary>
    /// <param name="x">The x offset relative to the player's center.</param>
    /// <param name="y">The y offset relative to the player's center.</param>
    void SetFocalPoint( int x, int y )
    {
        right_position = new Vector3(  x, y, depth );
        left_position  = new Vector3( -x, y, depth );
    }
}
