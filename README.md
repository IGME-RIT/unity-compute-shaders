Documentation Author: Alexander Amling

# Unity Compute Shaders

[part 2](https://github.com/IGME-RIT/unity-indirect-instanced-rendering)

*Unity Compute Shaders Part 3*

This demo builds off of where part 2 left off, and incorperates a compute shader that changes the transform of the object instances.

For the most part, the syntax of compute shaders in Unity is similar to the syntax of a typical surface shader since both are written in DX11 HLSL, but there are a few new concepts.

In Unity compute shaders, the functions of the shader are invoked by retrieving and executing the kernel of the desired function. A single compute shader can have up to 4 kernels, but for this demo we use just one. The shader requires a directive to compile each function. Ours looks likes this:
```
#pragma kernel Translate
...
[numthreads(64,1,1)]
void Translate (uint3 id : SV_DispatchThreadID)
{
	...
}
```
This secction of code has 2 other important details. The [numthreads()] statement, and uint3 id : SV_DispatchThreadID. Both of these relate to a rather complicated aspect of compute shaders. In short, numthreads allows you to specify the dimensions of the thread groups, and uint3 id: SV_DispatchThreadID is used to track the value of the current index (good for indexing into data). For our needs, we only need to iterate in a single dimension, and the x dimension is 64 because it is good practice to work around the minimum threadgroup size of AMD hardware; 64 (Nvidia is 32). It is a good rule of thumb that when working in a single dimension or with unrelated data points 64x1x1 is a good fit, when working with things like pixels in an image or points on a plane 8x8x1 is a good fit, and when working with values in 3D space 4x4x4 is a good fit (never use 1x1x1). Keep in mind, all of this completely depends on your specific application, and I highly recommend that you do some research on your own (see [microsoft documentation](https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/sm5-attributes-numthreads))
