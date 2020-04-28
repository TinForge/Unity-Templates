# Lerp
Provides static methods for different lerp functions.

[Source Tutorial](https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/)
##
![enter image description here](https://chicounity3d.files.wordpress.com/2014/05/interp-sinerp.png?w=300&h=185)

float t = currentLerpTime / lerpTime;
t = Mathf.Sin(t * Mathf.PI * 0.5f);
##
![enter image description here](https://chicounity3d.files.wordpress.com/2014/05/interp-coserp.png?w=300&h=185)

t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f)
##
![interp-quad](https://chicounity3d.files.wordpress.com/2014/05/interp-quad.png?w=300&h=185)

t = t*t
##
![interp-smooth](https://chicounity3d.files.wordpress.com/2014/05/interp-smooth.png?w=300&h=185)

t = t*t * (3f - 2f*t)
##
![interp-smoother](https://chicounity3d.files.wordpress.com/2014/05/interp-smoother.png?w=300&h=185)

t = t*t*t * (t * (6f*t - 15f) + 10f)
