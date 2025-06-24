# Pseudorandom Utilities
Here's a small collection of utility classes I use for random generation. Provided as is.

### Usage

``` c#
enum Thing {
  Apple,
  Banana,
  Orange,
  Lemon
}

public static void Example() {
  // serialized random has the same api as the regular c# random class,
  // except its state can be serialized as a string. if that's of no use,
  // feel free to replace it with the regular System.Random 
  var random = new SerializableRandom(0xf00f00);

  // multiple pseudorandom instances can share the same prng
  var didHit = new PseudorandomSingle(random, .1);
  var pickSomething = new PseudorandomBag<Thing>(random, 
    new [] { Thing.Apple , Thing.Banana, Thing.Orange, Thing.Lemon});
  
  // set this to a positive integer to put duplicates of everything into the bag
  // pickSomething.duplicates = 2;

  const int NumIterations = 20;
  for (var i = 0; i < NumIterations; i++) {
    Debug.Log($"did hit: {didHit.Roll()}, something: {pickSomething.Roll()}");
  }
}
```

### Expected output: 
```  
did hit: False, something: Orange
did hit: False, something: Lemon
did hit: False, something: Banana
did hit: False, something: Banana
did hit: False, something: Apple
did hit: True, something: Lemon
did hit: False, something: Orange
did hit: False, something: Apple
did hit: False, something: Apple
did hit: False, something: Lemon
did hit: True, something: Apple
did hit: False, something: Banana
did hit: False, something: Orange
did hit: False, something: Lemon
did hit: False, something: Orange
did hit: True, something: Banana
did hit: False, something: Apple
did hit: False, something: Lemon
did hit: False, something: Banana
did hit: False, something: Lemon
```
