using System;
using System.Collections.Generic;
using System.Text;

namespace csPixelGameEngineCore.Extensions
{
    public class GFX2D : PGEX
    {
        public GFX2D(PixelGameEngine pge) : base(pge)
        {
        }

        public void DrawSprite(Sprite sprite, Transform2D transform)
        {
            if (sprite == null)
                return;

            // Work out bounding rectangle of sprite
            float ex = 0.0f, ey = 0.0f;
            vec2d_f s = transform.Forward(0.0f, 0.0f);
            vec2d_f p = new vec2d_f(s);

            s.x = Math.Min(s.x, p.x);
            s.y = Math.Min(s.y, p.y);
            ex = Math.Max(ex, p.x);
            ey = Math.Max(ey, p.y);

            transform.Forward(sprite.Width, sprite.Height);
            s.x = Math.Min(s.x, p.x);
            s.y = Math.Min(s.y, p.y);
            ex = Math.Max(ex, p.x);
            ey = Math.Max(ey, p.y);

            p = transform.Forward(0.0f, sprite.Height);
            s.x = Math.Min(s.x, p.x);
            s.y = Math.Min(s.y, p.y);
            ex = Math.Max(ex, p.x);
            ey = Math.Max(ey, p.y);

            p = transform.Forward(sprite.Width, 0.0f);
            s.x = Math.Min(s.x, p.x);
            s.y = Math.Min(s.y, p.y);
            ex = Math.Max(ex, p.x);
            ey = Math.Max(ey, p.y);

            // Perform inversion of transform if required
            transform.Invert();

            float sx = s.x;
            float sy = s.y;
            if (ex < sx)
                PGEMath.Swap(ref ex, ref sx);
            if (ey < sy)
                PGEMath.Swap(ref ey, ref sy);
            s.x = sx;
            s.y = sy;

            // Iterate through render space, and sample Sprite from suitable texel location
            for (float i = s.x; i < ex; i++)
            {
                for (float j = s.y; j < ey; j++)
                {
                    vec2d_f o = transform.Backward(i, j);
                    pge.Draw((uint)i, (uint)j, sprite.GetPixel((uint)(o.x + 0.5f), (uint)(o.y + 0.5f)));
                }
            }
        }

        public class Transform2D
        {
            private float[,,] _matrix = new float[4,3,3];
			private int _targetMatrix;
            private int _sourceMatrix;
            private bool _dirty;

            public Transform2D()
            {
                Reset();
            }

            public void Reset()
            {
                _targetMatrix = 0;
                _sourceMatrix = 1;
                _dirty = true;

                // Columns Then Rows

                // Matrices 0 & 1 are used as swaps in Transform accumulation
                _matrix[0,0,0] = 1.0f; _matrix[0,1,0] = 0.0f; _matrix[0,2,0] = 0.0f;
                _matrix[0,0,1] = 0.0f; _matrix[0,1,1] = 1.0f; _matrix[0,2,1] = 0.0f;
                _matrix[0,0,2] = 0.0f; _matrix[0,1,2] = 0.0f; _matrix[0,2,2] = 1.0f;

                _matrix[1,0,0] = 1.0f; _matrix[1,1,0] = 0.0f; _matrix[1,2,0] = 0.0f;
                _matrix[1,0,1] = 0.0f; _matrix[1,1,1] = 1.0f; _matrix[1,2,1] = 0.0f;
                _matrix[1,0,2] = 0.0f; _matrix[1,1,2] = 0.0f; _matrix[1,2,2] = 1.0f;
            }

            public void Multiply()
            {
                for (int c = 0; c < 3; c++)
                {
                    for (int r = 0; r < 3; r++)
                    {
                        _matrix[_targetMatrix,c,r] = _matrix[2,0,r] * _matrix[_sourceMatrix,c,0] +
                                                     _matrix[2,1,r] * _matrix[_sourceMatrix,c,1] +
                                                     _matrix[2,2,r] * _matrix[_sourceMatrix,c,2];
                    }
                }

                PGEMath.Swap(_targetMatrix, _sourceMatrix);
                _dirty = true; // Any transform multiply dirties the inversion
            }

            public void Rotate(float fTheta)
            {
                // Construct Rotation Matrix
                _matrix[2,0,0] =  (float)Math.Cos(fTheta); _matrix[2,1,0] = (float)Math.Sin(fTheta); _matrix[2,2,0] = 0.0f;
                _matrix[2,0,1] = -(float)Math.Sin(fTheta); _matrix[2,1,1] = (float)Math.Cos(fTheta); _matrix[2,2,1] = 0.0f;
                _matrix[2,0,2] = 0.0f;                     _matrix[2,1,2] = 0.0f;                    _matrix[2,2,2] = 1.0f;
                Multiply();
            }

            public void Translate(float ox, float oy)
            {
                // Construct Translate Matrix
                _matrix[2,0,0] = 1.0f; _matrix[2,1,0] = 0.0f; _matrix[2,2,0] = ox;
                _matrix[2,0,1] = 0.0f; _matrix[2,1,1] = 1.0f; _matrix[2,2,1] = oy;
                _matrix[2,0,2] = 0.0f; _matrix[2,1,2] = 0.0f; _matrix[2,2,2] = 1.0f;
                Multiply();
            }

            public void Scale(float sx, float sy)
            {
                // Construct Scale Matrix
                _matrix[2,0,0] = sx;   _matrix[2,1,0] = 0.0f; _matrix[2,2,0] = 0.0f;
                _matrix[2,0,1] = 0.0f; _matrix[2,1,1] = sy;   _matrix[2,2,1] = 0.0f;
                _matrix[2,0,2] = 0.0f; _matrix[2,1,2] = 0.0f; _matrix[2,2,2] = 1.0f;
                Multiply();
            }

            public void Shear(float sx, float sy)
            {
		        // Construct Shear Matrix		
		        _matrix[2,0,0] = 1.0f; _matrix[2,1,0] = sx;   _matrix[2,2,0] = 0.0f;
		        _matrix[2,0,1] = sy;   _matrix[2,1,1] = 1.0f; _matrix[2,2,1] = 0.0f;
		        _matrix[2,0,2] = 0.0f; _matrix[2,1,2] = 0.0f; _matrix[2,2,2] = 1.0f;
		        Multiply();
            }

            public void Perspective(float ox, float oy)
            {
                // Construct Translate Matrix
                _matrix[2,0,0] = 1.0f; _matrix[2,1,0] = 0.0f; _matrix[2,2,0] = 0.0f;
                _matrix[2,0,1] = 0.0f; _matrix[2,1,1] = 1.0f; _matrix[2,2,1] = 0.0f;
                _matrix[2,0,2] = ox;   _matrix[2,1,2] = oy;   _matrix[2,2,2] = 1.0f;
                Multiply();
            }

            public vec2d_f Forward(float x, float y)
            {
                vec2d_f ret = new vec2d_f(
                    x * _matrix[_sourceMatrix,0,0] + y * _matrix[_sourceMatrix,1,0] + _matrix[_sourceMatrix,2,0],
                    x * _matrix[_sourceMatrix,0,1] + y * _matrix[_sourceMatrix,1,1] + _matrix[_sourceMatrix,2,1]
                );

                float out_z = x * _matrix[_sourceMatrix,0,2] + y * _matrix[_sourceMatrix,1,2] + _matrix[_sourceMatrix,2,2];
                if (out_z != 0)
                {
                    ret /= out_z;
                }

                return ret;
            }

            public vec2d_f Backward(float x, float y)
            {
                vec2d_f ret = new vec2d_f(
                    x * _matrix[3,0,0] + y * _matrix[3,1,0] + _matrix[3,2,0],
                    x * _matrix[3,0,1] + y * _matrix[3,1,1] + _matrix[3,2,1]
                );
                float out_z = x * _matrix[3,0,2] + y * _matrix[3,1,2] + _matrix[3,2,2];
                if (out_z != 0)
                {
                    ret /= out_z;
                }

                return ret;
            }

            public void Invert()
            {
                if (_dirty) // Obviously costly so only do if needed
                {
                    float det = _matrix[_sourceMatrix,0,0] * (_matrix[_sourceMatrix,1,1] * _matrix[_sourceMatrix,2,2] - _matrix[_sourceMatrix,1,2] * _matrix[_sourceMatrix,2,1]) -
                                _matrix[_sourceMatrix,1,0] * (_matrix[_sourceMatrix,0,1] * _matrix[_sourceMatrix,2,2] - _matrix[_sourceMatrix,2,1] * _matrix[_sourceMatrix,0,2]) +
                                _matrix[_sourceMatrix,2,0] * (_matrix[_sourceMatrix,0,1] * _matrix[_sourceMatrix,1,2] - _matrix[_sourceMatrix,1,1] * _matrix[_sourceMatrix,0,2]);

                    float idet = 1.0f / det;
                    _matrix[3,0,0] = (_matrix[_sourceMatrix,1,1] * _matrix[_sourceMatrix,2,2] - _matrix[_sourceMatrix,1,2] * _matrix[_sourceMatrix,2,1]) * idet;
                    _matrix[3,1,0] = (_matrix[_sourceMatrix,2,0] * _matrix[_sourceMatrix,1,2] - _matrix[_sourceMatrix,1,0] * _matrix[_sourceMatrix,2,2]) * idet;
                    _matrix[3,2,0] = (_matrix[_sourceMatrix,1,0] * _matrix[_sourceMatrix,2,1] - _matrix[_sourceMatrix,2,0] * _matrix[_sourceMatrix,1,1]) * idet;
                    _matrix[3,0,1] = (_matrix[_sourceMatrix,2,1] * _matrix[_sourceMatrix,0,2] - _matrix[_sourceMatrix,0,1] * _matrix[_sourceMatrix,2,2]) * idet;
                    _matrix[3,1,1] = (_matrix[_sourceMatrix,0,0] * _matrix[_sourceMatrix,2,2] - _matrix[_sourceMatrix,2,0] * _matrix[_sourceMatrix,0,2]) * idet;
                    _matrix[3,2,1] = (_matrix[_sourceMatrix,0,1] * _matrix[_sourceMatrix,2,0] - _matrix[_sourceMatrix,0,0] * _matrix[_sourceMatrix,2,1]) * idet;
                    _matrix[3,0,2] = (_matrix[_sourceMatrix,0,1] * _matrix[_sourceMatrix,1,2] - _matrix[_sourceMatrix,0,2] * _matrix[_sourceMatrix,1,1]) * idet;
                    _matrix[3,1,2] = (_matrix[_sourceMatrix,0,2] * _matrix[_sourceMatrix,1,0] - _matrix[_sourceMatrix,0,0] * _matrix[_sourceMatrix,1,2]) * idet;
                    _matrix[3,2,2] = (_matrix[_sourceMatrix,0,0] * _matrix[_sourceMatrix,1,1] - _matrix[_sourceMatrix,0,1] * _matrix[_sourceMatrix,1,0]) * idet;
                    _dirty = false;
                }
            }
        }
    }
}
