﻿/* 
* 
* Authors: 
*  Dmitry Kolchev <dmitrykolchev@msn.com>
*  
* Copyright (C) 2010 Dmitry Kolchev
*
* This sourcecode is licenced under The GNU Lesser General Public License
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
* OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
* NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
* DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
* OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
* USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Managed.Graphics.Direct2D;
using Managed.Graphics.DirectWrite;
using Managed.Graphics.Imaging;

namespace Managed.Graphics.Forms
{
    public partial class Direct2DWindow : Form
    {
        private Direct2DFactory _factory;
        private DirectWriteFactory _directWriteFactory;
        private WicImagingFactory _imagingFactory;
        private WindowRenderTarget _renderTarget;
        private bool _clearBackground = true;
        public Direct2DWindow()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque |
                ControlStyles.UserPaint, true);
            CreateDeviceIndependentResources();
            InitializeComponent();
            this.Disposed += Direct2DWindow_Disposed;
        }

        void Direct2DWindow_Disposed(object sender, EventArgs e)
        {
            CleanUpDeviceResources();
            CleanUpDeviceIndependentResources();
        }

        protected Direct2DFactory Direct2DFactory
        {
            get
            {
                return this._factory;
            }
        }

        protected DirectWriteFactory DirectWriteFactory
        {
            get
            {
                if (this._directWriteFactory == null)
                {
                    this._directWriteFactory = DirectWriteFactory.Create(DirectWriteFactoryType.Shared);
                }
                return this._directWriteFactory;
            }
        }

        protected WicImagingFactory ImagingFactory
        {
            get
            {
                if (this._imagingFactory == null)
                {
                    this._imagingFactory = WicImagingFactory.Create();
                }
                return this._imagingFactory;
            }
        }

        [DefaultValue(true)]
        public bool ClearBackground
        {
            get { return this._clearBackground; }
            set
            {
                if (this._clearBackground != value)
                {
                    this._clearBackground = value;
                    Invalidate();
                }
            }
        }

        protected WindowRenderTarget RenderTarget
        {
            get
            {
                return this._renderTarget;
            }
        }
        private void CreateDeviceIndependentResources()
        {
            this._factory = Direct2DFactory.CreateFactory(FactoryType.SingleThreaded, DebugLevel.None);
            OnCreateDeviceIndependentResources(this._factory);
        }
        private void CleanUpDeviceIndependentResources()
        {
            OnCleanUpDeviceIndependentResources();
            SafeDispose(ref this._imagingFactory);
            SafeDispose(ref this._directWriteFactory);
            SafeDispose(ref this._factory);
        }

        protected virtual void OnCleanUpDeviceIndependentResources()
        {
        }

        private void CleanUpDeviceResources()
        {
            if (this._renderTarget != null)
            {
                OnCleanUpDeviceResources();
                this._renderTarget.Dispose();
                this._renderTarget = null;
            }
        }

        protected virtual void OnCleanUpDeviceResources()
        {
        }

        private void CreateDeviceResources()
        {
            if (this._factory != null)
            {
                if (this._renderTarget == null)
                {
                    this._renderTarget = this._factory.CreateWindowRenderTarget(this);
                    OnCreateDeviceResources(this._renderTarget);
                }
            }
        }

        protected virtual void OnCreateDeviceResources(WindowRenderTarget renderTarget)
        {
        }

        protected virtual void OnCreateDeviceIndependentResources(Direct2DFactory factory)
        {
        }

        protected sealed override void OnPaint(PaintEventArgs e)
        {
            CreateDeviceResources();
            if(this._renderTarget != null)
                RenderInternal(this._renderTarget);
        }
        private void RenderInternal(WindowRenderTarget renderTarget)
        {
            renderTarget.BeginDraw();
            try
            {
                renderTarget.Transform = Matrix3x2.Identity;
                if (this.ClearBackground)
                    this._renderTarget.Clear(Color.FromRGB(this.BackColor.R, this.BackColor.G, this.BackColor.B));
                OnRender(renderTarget);
            }
            finally
            {
                if (!renderTarget.EndDraw())
                    CleanUpDeviceResources();
            }
        }

        public void Render()
        {
            OnPaint(null);
        }

        protected virtual void OnRender(WindowRenderTarget renderTarget)
        {
        }

        protected static void SafeDispose<T>(ref T d) where T: class, IDisposable
        {
            if (d != null)
            {
                ((IDisposable)d).Dispose();
                d = default(T);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.RenderTarget != null)
            {
                this.RenderTarget.Resize(new SizeU { Width = (uint)ClientSize.Width, Height = (uint)ClientSize.Height });
                Invalidate();
            }
        }
    }
}
