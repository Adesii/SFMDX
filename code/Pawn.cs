using Sandbox;
using System;
using System.Linq;

// - SFMDX -
// Source Filmmaker in S&box
// Licensed under the MIT License
// 
// Copyright (c) 2022 KiwifruitDev
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace SFMDX;

partial class Pawn : AnimatedEntity
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		base.Spawn();
		EnableDrawing = false;
        EnableShadowCasting = false;
	}

	/// <summary>
	/// Called every tick, clientside and serverside.
	/// </summary>
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;

		// build movement from the input values
		var movement = new Vector3( Input.Forward, Input.Left, 0 ).Normal;

		// rotate it to the direction we're facing
		Velocity = Rotation * movement;

		// apply some speed to it
		Velocity *= Input.Down( InputButton.Run ) ? 1000 : 200;

		// calculate velocity and apply to position
        Position += Velocity * Time.Delta;

		// If Attack1 was just pressed, spawn a puppet
		if ( !IsServer && Input.Pressed( InputButton.PrimaryAttack ) )
		{
			Puppet puppet = new();
			puppet.SetModel( "models/citizen/citizen.vmdl" );
			puppet.Position = EyePosition + EyeRotation.Forward * 40;
            // Reset pitch and roll for rotation
            Rotation rot = Rotation.LookAt( -( puppet.Position - EyePosition ).Normal );
            rot.x = 0;
            rot.y = 0;
			puppet.Rotation = rot;
		}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		// Update rotation every frame, to keep things smooth
		Rotation = Input.Rotation;
		EyeRotation = Rotation;
	}
}
