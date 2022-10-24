using System;

namespace RasterStudio.Models
{
    static public class EasingTool
    {
        /// <summary>
        /// Constant Pi.
        /// </summary>
        private const double PI = Math.PI;

        /// <summary>
        /// Constant Pi / 2.
        /// </summary>
        private const double HALFPI = Math.PI / 2.0f;

        /// <summary>
        /// Interpolate using the specified function.
        /// </summary>
        static public double Interpolate(double normalizedValue, EasingFunction function, EasingMode mode)
        {
            switch (function)
            {
                default:
                case EasingFunction.Linear:
                    return Linear(normalizedValue);
                case EasingFunction.Quadratic:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return QuadraticEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return QuadraticEaseIn(normalizedValue);
                        default:
                            return QuadraticEaseOut(normalizedValue);
                    }
                case EasingFunction.Quartic:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return QuarticEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return QuarticEaseIn(normalizedValue);
                        default:
                            return QuarticEaseOut(normalizedValue);
                    }
                case EasingFunction.Back:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return BackEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return BackEaseIn(normalizedValue);
                        default:
                            return BackEaseOut(normalizedValue);
                    }
                case EasingFunction.Bounce:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return BounceEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return BounceEaseIn(normalizedValue);
                        default:
                            return BounceEaseOut(normalizedValue);
                    }
                case EasingFunction.Circular:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return CircularEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return CircularEaseIn(normalizedValue);
                        default:
                            return CircularEaseOut(normalizedValue);
                    }
                case EasingFunction.Cubic:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return CubicEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return CubicEaseIn(normalizedValue);
                        default:
                            return CubicEaseOut(normalizedValue);
                    }
                case EasingFunction.Elastic:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return ElasticEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return ElasticEaseIn(normalizedValue);
                        default:
                            return ElasticEaseOut(normalizedValue);
                    }
                case EasingFunction.Exponential:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return ExponentialEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return ExponentialEaseIn(normalizedValue);
                        default:
                            return ExponentialEaseOut(normalizedValue);
                    }
                case EasingFunction.Quintic:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return QuinticEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return QuinticEaseIn(normalizedValue);
                        default:
                            return QuinticEaseOut(normalizedValue);
                    }
                case EasingFunction.Sine:
                    switch (mode)
                    {
                        case EasingMode.InOut:
                            return SineEaseInOut(normalizedValue);
                        case EasingMode.In:
                            return SineEaseIn(normalizedValue);
                        default:
                            return SineEaseOut(normalizedValue);
                    }

                case EasingFunction.None:
                    return 0;
            }
        }

        /// <summary>
        /// Modeled after the line y = x
        /// </summary>
        static public double Linear(double p)
        {
            return p;
        }

        /// <summary>
        /// Modeled after the parabola y = x^2
        /// </summary>
        static public double QuadraticEaseIn(double p)
        {
            return p * p;
        }

        /// <summary>
        /// Modeled after the parabola y = -x^2 + 2x
        /// </summary>
        static public double QuadraticEaseOut(double p)
        {
            return -(p * (p - 2));
        }

        /// <summary>
        /// Modeled after the piecewise quadratic
        /// y = (1/2)((2x)^2)             ; [0, 0.5)
        /// y = -(1/2)((2x-1)*(2x-3) - 1) ; [0.5, 1]
        /// </summary>
        static public double QuadraticEaseInOut(double p)
        {
            if (p < 0.5)
            {
                return 2 * p * p;
            }
            else
            {
                return (-2 * p * p) + (4 * p) - 1;
            }
        }

        /// <summary>
        /// Modeled after the cubic y = x^3
        /// </summary>
        static public double CubicEaseIn(double p)
        {
            return p * p * p;
        }

        /// <summary>
        /// Modeled after the cubic y = (x - 1)^3 + 1
        /// </summary>
        static public double CubicEaseOut(double p)
        {
            double f = (p - 1);
            return f * f * f + 1;
        }

        /// <summary>	
        /// Modeled after the piecewise cubic
        /// y = (1/2)((2x)^3)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^3 + 2) ; [0.5, 1]
        /// </summary>
        static public double CubicEaseInOut(double p)
        {
            if (p < 0.5)
            {
                return 4 * p * p * p;
            }
            else
            {
                double f = ((2 * p) - 2);
                return 0.5 * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after the quartic x^4
        /// </summary>
        static public double QuarticEaseIn(double p)
        {
            return p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quartic y = 1 - (x - 1)^4
        /// </summary>
        static public double QuarticEaseOut(double p)
        {
            double f = (p - 1);
            return f * f * f * (1 - p) + 1;
        }

        /// <summary>
        // Modeled after the piecewise quartic
        // y = (1/2)((2x)^4)        ; [0, 0.5)
        // y = -(1/2)((2x-2)^4 - 2) ; [0.5, 1]
        /// </summary>
        static public double QuarticEaseInOut(double p)
        {
            if (p < 0.5)
            {
                return 8 * p * p * p * p;
            }
            else
            {
                double f = (p - 1);
                return -8 * f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after the quintic y = x^5
        /// </summary>
        static public double QuinticEaseIn(double p)
        {
            return p * p * p * p * p;
        }

        /// <summary>
        /// Modeled after the quintic y = (x - 1)^5 + 1
        /// </summary>
        static public double QuinticEaseOut(double p)
        {
            double f = (p - 1);
            return f * f * f * f * f + 1;
        }

        /// <summary>
        /// Modeled after the piecewise quintic
        /// y = (1/2)((2x)^5)       ; [0, 0.5)
        /// y = (1/2)((2x-2)^5 + 2) ; [0.5, 1]
        /// </summary>
        static public double QuinticEaseInOut(double p)
        {
            if (p < 0.5)
            {
                return 16 * p * p * p * p * p;
            }
            else
            {
                double f = ((2 * p) - 2);
                return 0.5 * f * f * f * f * f + 1;
            }
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave
        /// </summary>
        static public double SineEaseIn(double p)
        {
            return Math.Sin((p - 1) * HALFPI) + 1;
        }

        /// <summary>
        /// Modeled after quarter-cycle of sine wave (different phase)
        /// </summary>
        static public double SineEaseOut(double p)
        {
            return Math.Sin(p * HALFPI);
        }

        /// <summary>
        /// Modeled after half sine wave
        /// </summary>
        static public double SineEaseInOut(double p)
        {
            return 0.5 * (1 - Math.Cos(p * PI));
        }

        /// <summary>
        /// Modeled after shifted quadrant IV of unit circle
        /// </summary>
        static public double CircularEaseIn(double p)
        {
            return 1 - Math.Sqrt(1 - (p * p));
        }

        /// <summary>
        /// Modeled after shifted quadrant II of unit circle
        /// </summary>
        static public double CircularEaseOut(double p)
        {
            return Math.Sqrt((2 - p) * p);
        }

        /// <summary>	
        /// Modeled after the piecewise circular function
        /// y = (1/2)(1 - Math.Sqrt(1 - 4x^2))           ; [0, 0.5)
        /// y = (1/2)(Math.Sqrt(-(2x - 3)*(2x - 1)) + 1) ; [0.5, 1]
        /// </summary>
        static public double CircularEaseInOut(double p)
        {
            if (p < 0.5)
            {
                return 0.5 * (1 - Math.Sqrt(1 - 4 * (p * p)));
            }
            else
            {
                return 0.5 * (Math.Sqrt(-((2 * p) - 3) * ((2 * p) - 1)) + 1);
            }
        }

        /// <summary>
        /// Modeled after the exponential function y = 2^(10(x - 1))
        /// </summary>
        static public double ExponentialEaseIn(double p)
        {
            return (p == 0.0f) ? p : Math.Pow(2, 10 * (p - 1));
        }

        /// <summary>
        /// Modeled after the exponential function y = -2^(-10x) + 1
        /// </summary>
        static public double ExponentialEaseOut(double p)
        {
            return (p == 1.0f) ? p : 1 - Math.Pow(2, -10 * p);
        }

        /// <summary>
        /// Modeled after the piecewise exponential
        /// y = (1/2)2^(10(2x - 1))         ; [0,0.5)
        /// y = -(1/2)*2^(-10(2x - 1))) + 1 ; [0.5,1]
        /// </summary>
        static public double ExponentialEaseInOut(double p)
        {
            if (p == 0.0 || p == 1.0) return p;

            if (p < 0.5)
            {
                return 0.5 * Math.Pow(2, (20 * p) - 10);
            }
            else
            {
                return -0.5 * Math.Pow(2, (-20 * p) + 10) + 1;
            }
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(13pi/2*x)*Math.Pow(2, 10 * (x - 1))
        /// </summary>
        static public double ElasticEaseIn(double p)
        {
            return Math.Sin(13 * HALFPI * p) * Math.Pow(2, 10 * (p - 1));
        }

        /// <summary>
        /// Modeled after the damped sine wave y = sin(-13pi/2*(x + 1))*Math.Pow(2, -10x) + 1
        /// </summary>
        static public double ElasticEaseOut(double p)
        {
            return Math.Sin(-13 * HALFPI * (p + 1)) * Math.Pow(2, -10 * p) + 1;
        }

        /// <summary>
        /// Modeled after the piecewise exponentially-damped sine wave:
        /// y = (1/2)*sin(13pi/2*(2*x))*Math.Pow(2, 10 * ((2*x) - 1))      ; [0,0.5)
        /// y = (1/2)*(sin(-13pi/2*((2x-1)+1))*Math.Pow(2,-10(2*x-1)) + 2) ; [0.5, 1]
        /// </summary>
        static public double ElasticEaseInOut(double p)
        {
            if (p < 0.5)
            {
                return 0.5 * Math.Sin(13 * HALFPI * (2 * p)) * Math.Pow(2, 10 * ((2 * p) - 1));
            }
            else
            {
                return 0.5 * (Math.Sin(-13 * HALFPI * ((2 * p - 1) + 1)) * Math.Pow(2, -10 * (2 * p - 1)) + 2);
            }
        }

        /// <summary>
        /// Modeled after the overshooting cubic y = x^3-x*sin(x*pi)
        /// </summary>
        static public double BackEaseIn(double p)
        {
            return p * p * p - p * Math.Sin(p * PI);
        }

        /// <summary>
        /// Modeled after overshooting cubic y = 1-((1-x)^3-(1-x)*sin((1-x)*pi))
        /// </summary>	
        static public double BackEaseOut(double p)
        {
            double f = (1 - p);
            return 1 - (f * f * f - f * Math.Sin(f * PI));
        }

        /// <summary>
        /// Modeled after the piecewise overshooting cubic function:
        /// y = (1/2)*((2x)^3-(2x)*sin(2*x*pi))           ; [0, 0.5)
        /// y = (1/2)*(1-((1-x)^3-(1-x)*sin((1-x)*pi))+1) ; [0.5, 1]
        /// </summary>
        static public double BackEaseInOut(double p)
        {
            if (p < 0.5)
            {
                double f = 2 * p;
                return 0.5 * (f * f * f - f * Math.Sin(f * PI));
            }
            else
            {
                double f = (1 - (2 * p - 1));
                return 0.5 * (1 - (f * f * f - f * Math.Sin(f * PI))) + 0.5;
            }
        }

        /// <summary>
        /// </summary>
        static public double BounceEaseIn(double p)
        {
            return 1 - BounceEaseOut(1 - p);
        }

        /// <summary>
        /// </summary>
        static public double BounceEaseOut(double p)
        {
            if (p < 4 / 11.0f)
            {
                return (121 * p * p) / 16.0f;
            }
            else if (p < 8 / 11.0f)
            {
                return (363 / 40.0f * p * p) - (99 / 10.0f * p) + 17 / 5.0f;
            }
            else if (p < 9 / 10.0f)
            {
                return (4356 / 361.0f * p * p) - (35442 / 1805.0f * p) + 16061 / 1805.0f;
            }
            else
            {
                return (54 / 5.0f * p * p) - (513 / 25.0f * p) + 268 / 25.0f;
            }
        }

        /// <summary>
        /// </summary>
        static public double BounceEaseInOut(double p)
        {
            if (p < 0.5)
            {
                return 0.5 * BounceEaseIn(p * 2);
            }
            else
            {
                return 0.5 * BounceEaseOut(p * 2 - 1) + 0.5;
            }
        }
    }

    public enum EasingFunction
    {
        None,
        Linear,
        Quadratic,
        Cubic,
        Quartic,
        Quintic,
        Sine,
        Circular,
        Exponential,
        Elastic,
        Back,
        Bounce
    }

    public enum EasingMode
    {
        Out = 0,
        In,
        InOut
    }
}
