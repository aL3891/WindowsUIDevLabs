﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//*********************************************************

using SamplesCommon.ImageLoader;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using CompositionAnimationToolkit;

namespace CompositionSampleGallery
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PropertySets : SamplePage
    {
        public PropertySets()
        {
            this.InitializeComponent();
        }

        public static string StaticSampleName { get { return "Expressions & PropertySets"; } }
        public override string SampleName { get { return StaticSampleName; } }
        public override string SampleDescription { get { return "Demonstrates how to use ExpressionAnimations and CompositionPropertySets to create a simple orbiting Visual."; } }
        public override string SampleCodeUri { get { return "http://go.microsoft.com/fwlink/p/?LinkID=761172"; } }

        private void SamplePage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(MyGrid).Compositor;
            IImageLoader imageLoader = ImageLoaderFactory.CreateImageLoader(compositor);
            ContainerVisual container = compositor.CreateContainerVisual();
            ElementCompositionPreview.SetElementChildVisual(MyGrid, container);



            //
            // Create a couple of SurfaceBrushes for the orbiters and center
            //

            CompositionSurfaceBrush redBrush = compositor.CreateSurfaceBrush();
            _redBallSurface = imageLoader.CreateManagedSurfaceFromUri(new Uri("ms-appx:///Samples/SDK 10586/PropertySets/RedBall.png"));
            redBrush.Surface = _redBallSurface.Surface;

            CompositionSurfaceBrush blueBrush = compositor.CreateSurfaceBrush();
            _blueBallSurface = imageLoader.CreateManagedSurfaceFromUri(new Uri("ms-appx:///Samples/SDK 10586/PropertySets/BlueBall.png"));
            blueBrush.Surface = _blueBallSurface.Surface;


            //
            // Create the center and orbiting sprites
            //

            SpriteVisual redSprite = compositor.CreateSpriteVisual();
            redSprite.Brush = redBrush;
            redSprite.Size = new Vector2(100f, 100f);
            redSprite.Offset = new Vector3(200f, 200f, 0f);
            container.Children.InsertAtTop(redSprite);

            SpriteVisual blueSprite = compositor.CreateSpriteVisual();
            blueSprite.Brush = blueBrush;
            blueSprite.Size = new Vector2(25f, 25f);
            blueSprite.Offset = new Vector3(0f, 0f, 0f);
            container.Children.InsertAtTop(blueSprite);

            //
            // Create the PropertySet.  This property bag contains all the value referenced in the expression.  We can
            // animation these property leading to the expression being re-evaluated per frame.
            //

            var propertySet = new
            {
                Rotation = 0f,
                CenterPointOffset = new Vector3(redSprite.Size.X / 2 - blueSprite.Size.X / 2, redSprite.Size.Y / 2 - blueSprite.Size.Y / 2, 0)
            };

            var props = blueSprite.CreateAnimation(r => r.Offset, c => redSprite.Offset + propertySet.CenterPointOffset
            + c.Vector3(c.Cos(c.ToRadians(propertySet.Rotation)) * 150, c.Sin(c.ToRadians(propertySet.Rotation)) * 75, 0)).Start().Properties;
            
            props.Get(() => propertySet).CreateAnimation(r => r.Rotation, 360).Duration(4000).Loop().EaseOut(compositor.CreateLinearEasingFunction()).Start();
            
            redSprite.CreateAnimation(r => r.Offset, new Vector3(125f, 50f, 0f), new Vector3(125f, 200f, 0f), new Vector3(125f, 50f, 0f)).Duration(4000).Loop().Start();
        }

        private void SamplePage_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _redBallSurface.Dispose();
            _blueBallSurface.Dispose();
        }

        private IManagedSurface _redBallSurface;
        private IManagedSurface _blueBallSurface;


        //class MyPropertySet : CompositionPropertySetWrapper
        //{
        //    public MyPropertySet(Compositor comp) : base(comp)
        //    {

        //    }
            
        //    public float Rotation { get { return GetScalar(); } set { SetValue(value); } }
        //    public Vector3 CenterPointOffset { get { return GetVector3(); } set { SetValue(value); } }
        //}
    }
}
