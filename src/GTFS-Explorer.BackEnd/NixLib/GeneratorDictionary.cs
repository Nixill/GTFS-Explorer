// This class is copied by its creator from
// https://github.com/ShadowFoxNixill/CS-NixLib/blob/master/CS-NixLib/src/Collections/GeneratorDictionary.cs
// and is not subject to the same license as the rest of the code of GTFS-Explorer.

using System;
using System.Collections.Generic;

namespace Nixill.Collections {
  /// <summary>
  /// A class that extends <a cref="Dictionary">Dictionary</a>, adding
  /// automatic generation of values from keys.
  /// </summary>
  /// <typeparam name="K">
  /// The type of the keys used in the dictionary.
  /// </typeparam>
  /// <typeparam name="V">
  /// The type of the values used in the dictionary.
  /// </typeparam>
  public class GeneratorDictionary<K, V> : Dictionary<K, V> {
    /// <summary>
    /// The <a cref="Generator">Generator</a> used by this
    /// <c>GeneratorDictionary</c>.
    /// </summary>
    public Generator<K, V> Generator { get; }

    /// <summary>
    /// Creates a new, empty Dictionary with the default initial capacity
    /// and default equality comparer.
    /// </summary>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(Generator<K, V> gen) {
      Generator = gen;
    }

    /// <summary>
    /// Copies an existing IDictionary into a new GeneratorDictionary.
    /// </summary>
    /// <param name="dictionary">The IDictionary to copy.</param>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(IDictionary<K, V> dictionary, Generator<K, V> gen) : base(dictionary) {
      Generator = gen;
    }

    /// <summary>
    /// Creates a new GeneratorDictionary using the given
    /// IEqualityComparer. 
    /// </summary>
    /// <param name="comparer">The IEqualityComparer to use.</param>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(IEqualityComparer<K> comparer, Generator<K, V> gen) : base(comparer) {
      Generator = gen;
    }

    /// <summary>
    /// Creates a new GeneratorDictionary using the given initial
    /// capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity to use.</param>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(int capacity, Generator<K, V> gen) : base(capacity) {
      Generator = gen;
    }

    /// <summary>
    /// Copies an existing IDictionary into a new GeneratorDictionary, but
    /// with a newly supplied IEqualityComparer. 
    /// </summary>
    /// <param name="dictionary">The IDictionary to copy.</param>
    /// <param name="comparer">The IEqualityComparer to use.</param>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(IDictionary<K, V> dictionary, IEqualityComparer<K> comparer, Generator<K, V> gen) : base(dictionary, comparer) {
      Generator = gen;
    }

    /// <summary>
    /// Creates a new GeneratorDictionary using the given initial capacity
    /// and IEqualityComparer.
    /// </summary>
    /// <param name="capacity">The initial capacity to use.</param>
    /// <param name="comparer">The IEqualityComparer to use.</param>
    /// <param name="gen">The Generator to use.</param>
    public GeneratorDictionary(int capacity, IEqualityComparer<K> comparer, Generator<K, V> gen) : base(capacity, comparer) {
      Generator = gen;
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    ///
    /// If the specified key is not found, a get operation automatically
    /// generates a value, sets it to that key, and returns it.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    public new V this[K key] {
      get {
        try {
          return base[key];
        }
        catch (KeyNotFoundException) {
          return Add(key);
        }
      }
      set => base[key] = value;
    }

    /// <summary>
    /// Adds the specified key, and an automatically-generated value, to
    /// the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    public V Add(K key) {
      if (key == null) {
        throw new ArgumentNullException("Null keys cannot be added to GeneratorDictionaries.");
      }
      else if (ContainsKey(key)) {
        throw new ArgumentException("Key " + key.ToString() + " already exists in map.");
      }
      else {
        V val = Generator.Generate(key);
        base[key] = val;
        return val;
      }
    }

    /// <summary>
    /// Whether or not any value can be generated for a key without
    /// throwing an exception.
    ///
    /// See <see  cref="Generator.CanGenerateFrom(K)" /> for return value
    /// details.
    ///
    /// This method does not check current dictionary contents. In
    /// particular, it may return <c>true</c> for keys already contained
    /// in the dictionary.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    public bool? CanGenerateForKey(K key) => (key == null) ? false : Generator.CanGenerateFrom(key);

    /// <summary>
    /// Whether or not a specific value can be generated immediately.
    /// 
    /// See <see cref="Generator.CanGenerate(V)" /> for return value
    /// details.
    ///
    /// This method does not check current dictionary contents. In
    /// particular, it may return <c>true</c> for values that can only be
    /// generated from keys already contained in the dictionary, and may
    /// also return <c>false</c> for values already contained in the
    /// dictionary.
    /// </summary>
    /// <param name="value">The value to check for.</param>
    public bool? CanGenerateValue(V val) => Generator.CanGenerate(val);
  }

  /// <summary>
  /// A class used by GeneratorDictionary to create values for given keys.
  /// </summary>
  public abstract class Generator<K, V> {
    /// <summary>
    /// Returns a value for the given key.
    /// </summary>
    public abstract V Generate(K key);

    /// <summary>
    /// Whether or not any value can be generated for a key without
    /// throwing an exception.
    ///
    /// More specifically, this method checks whether the <i>current
    /// state</i> of the Generator can accept this key and return a value.
    ///
    /// A return value of <c>true</c> means an immediate call to Generate
    /// with the given key will definitely succeed. A return value of
    /// <c>false</c> means an immediate call to Generate with the given
    /// key will definitely throw an exception. A return value of
    /// <c>null</c> means no such promises can be made either way.
    ///
    /// Any value returned by this method is invalidated if the internal
    /// state of this Generator changes after it is called.
    ///
    /// When overriding this method, <c>null</c> is always a safe return
    /// value.
    /// </summary>
    public bool? CanGenerateFrom(K key) => null;

    /// <summary>
    /// Whether or not a specific value can be generated immediately.
    ///
    /// More specifically, whether there is any key that would cause the
    /// given value (or another value for which <c>Equals()</c>, with the
    /// supplied value, returns true) to be generated immediately with the
    /// <i>current state</i> of the Generator.
    ///
    /// A return value of <c>true</c> means that there exists at least one
    /// key that could cause the value to be generated on the next call to
    /// <c>Generate()</c>. A return value of <c>false</c> means that there
    /// exists no key that could possibly cause the value to be generated.
    /// A return value of <c>null</c> means neither of the above promises
    /// can be made.
    ///
    /// Any value returned by this method is invalidated if the internal
    /// state of this Generator changes after it is called.
    ///
    /// When overriding this method, <c>null</c> is always a safe return
    /// value.
    /// </summary>
    /// <param name="value">The value to check for.</param>
    public bool? CanGenerate(V val) => null;
  }
}