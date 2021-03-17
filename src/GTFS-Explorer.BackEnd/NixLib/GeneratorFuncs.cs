// This class is copied by its creator from
// https://github.com/ShadowFoxNixill/CS-NixLib/blob/master/CS-NixLib/src/Collections/GeneratorFuncs.cs
// and is not subject to the same license as the rest of the code of GTFS-Explorer.

using System;

namespace Nixill.Collections
{
  /// <summary>
  /// A Generator that always returns the same value (supplied at
  /// construction).
  /// </summary>
  public class SingleValueGenerator<K, V> : Generator<K, V>
  {
    /// <summary>
    /// The value this Generator returns.
    /// </summary>
    public V Val { get; }

    /// <summary>
    /// Creates a new SingleValueGenerator with a given value.
    /// </summary>
    public SingleValueGenerator(V val)
    {
      Val = val;
    }

    /// <summary>
    /// Returns the initially supplied value, regardless of key.
    /// </summary>
    public override V Generate(K key) => Val;

    /// <summary>
    /// Returns whether or not a value can be generated for the given key
    /// without throwing an exception. This is <c>true</c> for any key.
    /// </summary>
    public new bool? CanGenerateFrom(K key) => true;

    /// <summary>
    /// Returns whether or not the given value can be returned by the
    /// Generator. This is always <c>false</c>, unless the given value is
    /// the one supplied at construction.
    /// </summary>
    public new bool? CanGenerate(V val)
    {
      if (Val == null) return val == null;
      else return Val.Equals(val);
    }
  }

  /// <summary>
  /// A Generator that returns the key as its own value.
  /// </summary>
  public class EchoGenerator<T> : Generator<T, T>
  {
    /// <summary>
    /// Returns (echoes) the supplied value.
    /// </summary>
    public override T Generate(T key) => key;

    /// <summary>
    /// Returns whether or not a value can be generated for the given key
    /// without throwing an exception. This is <c>true</c> for any key.
    /// </summary>
    public new bool? CanGenerateFrom(T key) => true;

    /// <summary>
    /// Returns whether or not the given value can be generated for the
    /// right key. This is <c>true</c> for any value.
    /// </summary>
    public new bool? CanGenerate(T val) => true;
  }

  /// <summary>
  /// A Generator that returns incrementally counted keys, starting with
  /// zero.
  /// </summary>
  public class CountingGenerator<K> : Generator<K, int>
  {
    /// <summary>
    /// The next number that will be Generated.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Creates a new CountingGenerator.
    /// </summary>
    public CountingGenerator()
    {
      Count = 0;
    }

    /// <summary>
    /// Returns the next integer in sequence.
    /// </summary>
    public override int Generate(K key) => Count++;

    /// <summary>
    /// Returns whether or not a value can be generated for the given key
    /// without throwing an exception. This is <c>true</c> for any key.
    /// </summary>
    public new bool? CanGenerateFrom(K key) => true;

    /// <summary>
    /// Returns whether or not there exists a key to generate a given
    /// value. This is <c>false</c> for any value except the next number
    /// in sequence.
    /// </summary>
    public new bool? CanGenerate(int val)
    {
      return Count <= val;
    }
  }

  /// <summary>
  /// A Generator based on an arbitrary Func.
  /// </summary>
  public class FuncGenerator<K, V> : Generator<K, V>
  {
    /// <summary>
    /// The Func that is used to perform the actual generation of values.
    /// </summary>
    public Func<K, V> GeneratingFunc { get; }

    /// <summary>
    /// The Func that checks whether or not a value can be generated for a
    /// given key.
    /// </summary>
    public Func<K, bool?> KeyCheckFunc { get; }

    /// <summary>
    /// The Func that checks whether or not any key can generate a given
    /// value.
    /// </summary>
    public Func<V, bool?> ValCheckFunc { get; }

    /// <summary>
    /// Creates a new FuncGenerator with a given generating func, and the
    /// default check functions (which will always return null).
    /// </summary>
    public FuncGenerator(Func<K, V> func) : this(func, (key) => null, (val) => null) { }

    /// <summary>
    /// Creates a new FuncGenerator with given generating and checking
    /// funcs.
    /// </summary>
    public FuncGenerator(Func<K, V> func, Func<K, bool?> keyCheck, Func<V, bool?> valCheck)
    {
      GeneratingFunc = func;
      KeyCheckFunc = keyCheck;
      ValCheckFunc = valCheck;
    }

    public override V Generate(K key) => GeneratingFunc.Invoke(key);
    public new bool? CanGenerateFrom(K key) => KeyCheckFunc.Invoke(key);
    public new bool? CanGenerate(V val) => ValCheckFunc.Invoke(val);
  }

  /// <summary>
  /// A Generator that generates values which are the HashCodes of the
  /// keys used to generate them.
  /// </summary>
  public class HashCodeGenerator<K> : Generator<K, int>
  {
    /// <summary>
    /// Returns the HashCode of the key.
    /// </summary>
    public override int Generate(K key) => key.GetHashCode();
  }

  /// <summary>
  /// A Generator that generates values which are the string
  /// representations of the keys used to generate them.
  public class ToStringGenerator<K> : Generator<K, string>
  {
    /// <summary>
    /// Returns the string representation of the key.
    /// </summary>
    public override string Generate(K key) => key.ToString();
  }
}