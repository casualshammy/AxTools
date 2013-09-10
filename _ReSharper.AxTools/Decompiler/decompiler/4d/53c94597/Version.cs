// Type: System.Version
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
  /// <summary>
  /// Представляет номер версии построения среды CLR.Данный класс не может наследоваться.
  /// </summary>
  /// <filterpriority>1</filterpriority>
  [ComVisible(true)]
  [Serializable]
  public sealed class Version : ICloneable, IComparable, IComparable<Version>, IEquatable<Version>
  {
    private int _Build = -1;
    private int _Revision = -1;
    private int _Major;
    private int _Minor;

    /// <summary>
    /// Возвращает значение компонента текущего объекта <see cref="T:System.Version"/>, представляющего в номере версии основной номер.
    /// </summary>
    /// 
    /// <returns>
    /// Основной номер версии.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public int Major
    {
      get
      {
        return this._Major;
      }
    }

    /// <summary>
    /// Возвращает значение компонента текущего объекта <see cref="T:System.Version"/>, представляющего в номере версии дополнительный номер.
    /// </summary>
    /// 
    /// <returns>
    /// Дополнительный номер версии.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public int Minor
    {
      get
      {
        return this._Minor;
      }
    }

    /// <summary>
    /// Возвращает значение компонента текущего объекта <see cref="T:System.Version"/>, представляющего в номере версии номер построения.
    /// </summary>
    /// 
    /// <returns>
    /// Номер построения или значение -1, если номер сборки не определен.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public int Build
    {
      get
      {
        return this._Build;
      }
    }

    /// <summary>
    /// Возвращает значение компонента текущего объекта <see cref="T:System.Version"/>, представляющего в номере версии номер редакции.
    /// </summary>
    /// 
    /// <returns>
    /// Номер редакции или значение -1, если номер редакции не определен.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public int Revision
    {
      get
      {
        return this._Revision;
      }
    }

    /// <summary>
    /// Возвращает старшие 16 разрядов номера редакции.
    /// </summary>
    /// 
    /// <returns>
    /// 16-битовое целое число со знаком.
    /// </returns>
    public short MajorRevision
    {
      get
      {
        return (short) (this._Revision >> 16);
      }
    }

    /// <summary>
    /// Возвращает младшие 16 разрядов номера редакции.
    /// </summary>
    /// 
    /// <returns>
    /// 16-битовое целое число со знаком.
    /// </returns>
    public short MinorRevision
    {
      get
      {
        return (short) (this._Revision & (int) ushort.MaxValue);
      }
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Version"/> с помощью указанных основного и дополнительного номеров версии, номера построения и номера редакции.
    /// </summary>
    /// <param name="major">Основной номер версии. </param><param name="minor">Дополнительный номер версии. </param><param name="build">Номер построения. </param><param name="revision">Номер редакции. </param><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="major"/>, <paramref name="minor"/>, <paramref name="build"/> или <paramref name="revision"/> меньше нуля. </exception>
    public Version(int major, int minor, int build, int revision)
    {
      if (major < 0)
        throw new ArgumentOutOfRangeException("major", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      if (minor < 0)
        throw new ArgumentOutOfRangeException("minor", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      if (build < 0)
        throw new ArgumentOutOfRangeException("build", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      if (revision < 0)
        throw new ArgumentOutOfRangeException("revision", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      this._Major = major;
      this._Minor = minor;
      this._Build = build;
      this._Revision = revision;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Version"/> с помощью указанных основного и дополнительного номеров версии и номера построения.
    /// </summary>
    /// <param name="major">Основной номер версии. </param><param name="minor">Дополнительный номер версии. </param><param name="build">Номер построения. </param><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра равно <paramref name="major"/>, <paramref name="minor"/> или <paramref name="build"/> меньше нуля. </exception>
    public Version(int major, int minor, int build)
    {
      if (major < 0)
        throw new ArgumentOutOfRangeException("major", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      if (minor < 0)
        throw new ArgumentOutOfRangeException("minor", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      if (build < 0)
        throw new ArgumentOutOfRangeException("build", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      this._Major = major;
      this._Minor = minor;
      this._Build = build;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Version"/> с помощью указанных основного и дополнительного номеров версии.
    /// </summary>
    /// <param name="major">Основной номер версии. </param><param name="minor">Дополнительный номер версии. </param><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="major"/> или <paramref name="minor"/> меньше нуля. </exception>
    public Version(int major, int minor)
    {
      if (major < 0)
        throw new ArgumentOutOfRangeException("major", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      if (minor < 0)
        throw new ArgumentOutOfRangeException("minor", Environment.GetResourceString("ArgumentOutOfRange_Version"));
      this._Major = major;
      this._Minor = minor;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Version"/>, используя указанную строку.
    /// </summary>
    /// <param name="version">Строка, содержащая основной и дополнительный номер версии, номер построения и номер редакции, в которой каждое число отделено точкой (.). </param><exception cref="T:System.ArgumentException">В параметре <paramref name="version"/> содержится менее двух или более четырех компонентов. </exception><exception cref="T:System.ArgumentNullException"><paramref name="version"/> имеет значение null; </exception><exception cref="T:System.ArgumentOutOfRangeException">Компонент, представляющий основной или дополнительный номера версии, номер построения или редакции, меньше нуля. </exception><exception cref="T:System.FormatException">По меньшей мере, один компонент параметра <paramref name="version"/> не распознан как десятичное число. </exception><exception cref="T:System.OverflowException">По меньшей мере, один компонент параметра <paramref name="version"/> представляет число, превышающее <see cref="F:System.Int32.MaxValue"/>.</exception>
    public Version(string version)
    {
      Version version1 = Version.Parse(version);
      this._Major = version1.Major;
      this._Minor = version1.Minor;
      this._Build = version1.Build;
      this._Revision = version1.Revision;
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="T:System.Version"/>.
    /// </summary>
    public Version()
    {
      this._Major = 0;
      this._Minor = 0;
    }

    /// <summary>
    /// Определяет, равны ли между собой два указанных объекта <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Если значение свойства <paramref name="v1"/> равно <paramref name="v2"/>, значение true; в противном случае — значение false.
    /// </returns>
    /// <param name="v1">Первый объект <see cref="T:System.Version"/>. </param><param name="v2">Второй объект <see cref="T:System.Version"/>. </param><filterpriority>3</filterpriority>
    public static bool operator ==(Version v1, Version v2)
    {
      if (object.ReferenceEquals((object) v1, (object) null))
        return object.ReferenceEquals((object) v2, (object) null);
      else
        return v1.Equals(v2);
    }

    /// <summary>
    /// Определяет, действительно ли два заданных объекта <see cref="T:System.Version"/> не равны.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если значения параметров <paramref name="v1"/> и <paramref name="v2"/> не равны; в противном случае — значение false.
    /// </returns>
    /// <param name="v1">Первый объект <see cref="T:System.Version"/>. </param><param name="v2">Второй объект <see cref="T:System.Version"/>. </param><filterpriority>3</filterpriority>
    public static bool operator !=(Version v1, Version v2)
    {
      return !(v1 == v2);
    }

    /// <summary>
    /// Определяет, меньше ли значение первого указанного объекта <see cref="T:System.Version"/>, чем значение второго указанного объекта <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если значение <paramref name="v1"/> меньше значения <paramref name="v2"/>; в противном случае — значение false.
    /// </returns>
    /// <param name="v1">Первый объект <see cref="T:System.Version"/>. </param><param name="v2">Второй объект <see cref="T:System.Version"/>. </param><exception cref="T:System.ArgumentNullException"><paramref name="v1"/> имеет значение null; </exception><filterpriority>3</filterpriority>
    public static bool operator <(Version v1, Version v2)
    {
      if (v1 == null)
        throw new ArgumentNullException("v1");
      else
        return v1.CompareTo(v2) < 0;
    }

    /// <summary>
    /// Определяет, является ли значение первого указанного объекта <see cref="T:System.Version"/> меньшим или равным значению второго указанного объекта <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если значение <paramref name="v1"/> меньше или равно значению <paramref name="v2"/>; в противном случае — значение false.
    /// </returns>
    /// <param name="v1">Первый объект <see cref="T:System.Version"/>. </param><param name="v2">Второй объект <see cref="T:System.Version"/>. </param><exception cref="T:System.ArgumentNullException"><paramref name="v1"/> имеет значение null; </exception><filterpriority>3</filterpriority>
    public static bool operator <=(Version v1, Version v2)
    {
      if (v1 == null)
        throw new ArgumentNullException("v1");
      else
        return v1.CompareTo(v2) <= 0;
    }

    /// <summary>
    /// Определяет, больше ли значение первого указанного объекта <see cref="T:System.Version"/>, чем значение второго указанного объекта <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если значение <paramref name="v1"/> больше значения <paramref name="v2"/>; в противном случае — значение false.
    /// </returns>
    /// <param name="v1">Первый объект <see cref="T:System.Version"/>. </param><param name="v2">Второй объект <see cref="T:System.Version"/>. </param><filterpriority>3</filterpriority>
    public static bool operator >(Version v1, Version v2)
    {
      return v2 < v1;
    }

    /// <summary>
    /// Определяет, является ли значение первого указанного объекта <see cref="T:System.Version"/> большим или равным значению второго указанного объекта <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true, если значение <paramref name="v1"/> больше или равно значению <paramref name="v2"/>; в противном случае — значение false.
    /// </returns>
    /// <param name="v1">Первый объект <see cref="T:System.Version"/>. </param><param name="v2">Второй объект <see cref="T:System.Version"/>. </param><filterpriority>3</filterpriority>
    public static bool operator >=(Version v1, Version v2)
    {
      return v2 <= v1;
    }

    /// <summary>
    /// Возвращает новый объект <see cref="T:System.Version"/>, значение которого совпадает со значением текущего объекта <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Новый объект <see cref="T:System.Object"/>, значение которого является копией текущего объекта <see cref="T:System.Version"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public object Clone()
    {
      return (object) new Version()
      {
        _Major = this._Major,
        _Minor = this._Minor,
        _Build = this._Build,
        _Revision = this._Revision
      };
    }

    /// <summary>
    /// Сравнивает текущий объект <see cref="T:System.Version"/> с заданным объектом и возвращает сведения об их относительных значениях.
    /// </summary>
    /// 
    /// <returns>
    /// Знаковое целое число, которое определяет относительные значения двух объектов, как показано в следующей таблице.Возвращаемое значение Описание Меньше нуля В текущем объекте <see cref="T:System.Version"/> указана версия, предшествующая версии, указанной в параметре <paramref name="version"/>. Нуль Версия текущего объекта <see cref="T:System.Version"/> совпадает с версией, указанной в параметре <paramref name="version"/>. Больше нуля В текущем объекте <see cref="T:System.Version"/> указана версия, следующая за версией, указанной в параметре <paramref name="version"/>.— или — Параметр <paramref name="version"/> имеет значение null.
    /// </returns>
    /// <param name="version">Объект для сравнения или значение null. </param><exception cref="T:System.ArgumentException">Параметр <paramref name="version"/> не является параметром типа <see cref="T:System.Version"/>. </exception><filterpriority>1</filterpriority>
    public int CompareTo(object version)
    {
      if (version == null)
        return 1;
      Version version1 = version as Version;
      if (version1 == (Version) null)
        throw new ArgumentException(Environment.GetResourceString("Arg_MustBeVersion"));
      if (this._Major != version1._Major)
        return this._Major > version1._Major ? 1 : -1;
      else if (this._Minor != version1._Minor)
        return this._Minor > version1._Minor ? 1 : -1;
      else if (this._Build != version1._Build)
      {
        return this._Build > version1._Build ? 1 : -1;
      }
      else
      {
        if (this._Revision == version1._Revision)
          return 0;
        return this._Revision > version1._Revision ? 1 : -1;
      }
    }

    /// <summary>
    /// Сравнивает текущий объект <see cref="T:System.Version"/> с заданным объектом <see cref="T:System.Version"/> и возвращает сведения об их относительных значениях.
    /// </summary>
    /// 
    /// <returns>
    /// Знаковое целое число, которое определяет относительные значения двух объектов, как показано в следующей таблице.Возвращаемое значение Описание Меньше нуля В текущем объекте <see cref="T:System.Version"/> указана версия, предшествующая версии, указанной в параметре <paramref name="value"/>. Нуль Версия текущего объекта <see cref="T:System.Version"/> совпадает с версией, указанной в параметре <paramref name="value"/>. Больше нуля В текущем объекте <see cref="T:System.Version"/> указана версия, следующая за версией, указанной в параметре <paramref name="value"/>. — или —Параметр <paramref name="value"/> имеет значение null.
    /// </returns>
    /// <param name="value">Объект <see cref="T:System.Version"/>, сравниваемый с текущим объектом <see cref="T:System.Version"/>, или значение null.</param><filterpriority>1</filterpriority>
    public int CompareTo(Version value)
    {
      if (value == (Version) null)
        return 1;
      if (this._Major != value._Major)
        return this._Major > value._Major ? 1 : -1;
      else if (this._Minor != value._Minor)
        return this._Minor > value._Minor ? 1 : -1;
      else if (this._Build != value._Build)
      {
        return this._Build > value._Build ? 1 : -1;
      }
      else
      {
        if (this._Revision == value._Revision)
          return 0;
        return this._Revision > value._Revision ? 1 : -1;
      }
    }

    /// <summary>
    /// Возвращает значение, позволяющее определить, равен ли текущий объект <see cref="T:System.Version"/> указанному.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true если текущий объект <see cref="T:System.Version"/> и объект <paramref name="obj"/> оба являются объектами <see cref="T:System.Version"/>, и все компоненты текущего объекта <see cref="T:System.Version"/> совпадают с соответствующими компонентами объекта <paramref name="obj"/>; в противном случае — значение false.
    /// </returns>
    /// <param name="obj">Объект, сравниваемый с текущим объектом <see cref="T:System.Version"/>, или значение null. </param><filterpriority>1</filterpriority>
    public override bool Equals(object obj)
    {
      Version version = obj as Version;
      return !(version == (Version) null) && this._Major == version._Major && (this._Minor == version._Minor && this._Build == version._Build) && this._Revision == version._Revision;
    }

    /// <summary>
    /// Возвращает значение, позволяющее определить, представляют ли текущий объект <see cref="T:System.Version"/> и заданный объект <see cref="T:System.Version"/> одно и то же значение.
    /// </summary>
    /// 
    /// <returns>
    /// Значение true если все компоненты текущего объекта <see cref="T:System.Version"/> совпадают с соответствующими компонентами параметра <paramref name="obj"/>; в противном случае — значение false.
    /// </returns>
    /// <param name="obj">Объект <see cref="T:System.Version"/>, сравниваемый с текущим объектом <see cref="T:System.Version"/>, или значение null.</param><filterpriority>1</filterpriority>
    public bool Equals(Version obj)
    {
      return !(obj == (Version) null) && this._Major == obj._Major && (this._Minor == obj._Minor && this._Build == obj._Build) && this._Revision == obj._Revision;
    }

    /// <summary>
    /// Возвращает хэш-код для текущего объекта <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Хэш-код в виде 32-битового целого числа со знаком.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override int GetHashCode()
    {
      return 0 | (this._Major & 15) << 28 | (this._Minor & (int) byte.MaxValue) << 20 | (this._Build & (int) byte.MaxValue) << 12 | this._Revision & 4095;
    }

    /// <summary>
    /// Преобразует значение текущего объекта <see cref="T:System.Version"/> в эквивалентное ему представление <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Представление <see cref="T:System.String"/> для значений компонентов текущего объекта <see cref="T:System.Version"/>, соответствующих основному и дополнительному номерам версий, номеру построения и номеру редакции, согласно приведенному ниже формату.Все компоненты разделены точкой (.).Квадратные скобки ("[" и "]") указывают на компонент, который не будет отображаться в возвращаемом значении, если он не определен.ОсновнойНомерВерсии.ДополнительныйНомерВерсии[.НомерПостроения[.НомерРедакции]] Например, при создании объекта <see cref="T:System.Version"/> с помощью конструктора Version(1,1) возвращается строка "1.1".Например, при создании объекта <see cref="T:System.Version"/> с помощью конструктора Version(1,3,4,2) возвращается строка "1.3.4.2".
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public override string ToString()
    {
      if (this._Build == -1)
        return this.ToString(2);
      if (this._Revision == -1)
        return this.ToString(3);
      else
        return this.ToString(4);
    }

    /// <summary>
    /// Преобразует значение текущего объекта <see cref="T:System.Version"/> в эквивалентное ему представление <see cref="T:System.String"/>.Заданное количество обозначает число возвращаемых компонент.
    /// </summary>
    /// 
    /// <returns>
    /// Представление <see cref="T:System.String"/> для значений компонентов текущего объекта <see cref="T:System.Version"/>, соответствующих основному и дополнительному номерам версий, номеру построения и номеру редакции, в котором все значения разделены точкой (.).Параметр <paramref name="fieldCount"/> определяет количество возвращаемых компонентов.fieldCount Возвращаемое значение 0 Пустая строка (""). 1 основной номер версии 2 ОсновнойНомерВерсии.ДополнительныйНомерВерсии 3 ОсновнойНомерВерсии.ДополнительныйНомерВерсии.НомерПостроения 4 ОсновнойНомерВерсии.ДополнительныйНомерВерсии.НомерПостроения.НомерРедакции Например, при создании объекта <see cref="T:System.Version"/> с помощь конструктора Version(1,3,5), ToString(2) возвращается строка "1.3", а метод ToString(4) выбрасывает исключение.
    /// </returns>
    /// <param name="fieldCount">Число возвращаемых компонентов.Значение параметра <paramref name="fieldCount"/> лежит в диапазоне от 0 до 4.</param><exception cref="T:System.ArgumentException">Значение параметра <paramref name="fieldCount"/> меньше 0 или больше 4.— или — Значение параметра <paramref name="fieldCount"/> превышает число компонентов, определенных в текущем объекте <see cref="T:System.Version"/>. </exception><filterpriority>1</filterpriority>
    public string ToString(int fieldCount)
    {
      switch (fieldCount)
      {
        case 0:
          return string.Empty;
        case 1:
          return string.Concat((object) this._Major);
        case 2:
          return (string) (object) this._Major + (object) "." + (string) (object) this._Minor;
        default:
          if (this._Build == -1)
            throw new ArgumentException(Environment.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", (object) "0", (object) "2"), "fieldCount");
          else if (fieldCount == 3)
            return (string) (object) this._Major + (object) "." + (string) (object) this._Minor + "." + (string) (object) this._Build;
          else if (this._Revision == -1)
            throw new ArgumentException(Environment.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", (object) "0", (object) "3"), "fieldCount");
          else if (fieldCount == 4)
            return (string) (object) this.Major + (object) "." + (string) (object) this._Minor + "." + (string) (object) this._Build + "." + (string) (object) this._Revision;
          else
            throw new ArgumentException(Environment.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper", (object) "0", (object) "4"), "fieldCount");
      }
    }

    /// <summary>
    /// Преобразует строковое представление номера версии в эквивалентный объект <see cref="T:System.Version"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Объект, эквивалентный номеру версии, который задается параметром <paramref name="input"/>.
    /// </returns>
    /// <param name="input">Строка, содержащая преобразуемый номер версии.</param><exception cref="T:System.ArgumentNullException"><paramref name="input"/> имеет значение null;</exception><exception cref="T:System.ArgumentException">В параметре <paramref name="input"/> содержится менее двух или более четырех компонентов версии.</exception><exception cref="T:System.ArgumentOutOfRangeException">По крайней мере один из компонентов в <paramref name="input"/> меньше нуля.</exception><exception cref="T:System.FormatException">По крайней мере один из компонентов в <paramref name="input"/> не является целым числом.</exception><exception cref="T:System.OverflowException">По меньшей мере, один компонент в <paramref name="input"/> представляет число, превышающее <see cref="F:System.Int32.MaxValue"/>.</exception>
    public static Version Parse(string input)
    {
      if (input == null)
        throw new ArgumentNullException("input");
      Version.VersionResult result = new Version.VersionResult();
      result.Init("input", true);
      if (!Version.TryParseVersion(input, ref result))
        throw result.GetVersionParseException();
      else
        return result.m_parsedVersion;
    }

    /// <summary>
    /// Предпринимает попытку преобразования строкового представления номера версии в эквивалентный объект <see cref="T:System.Version"/> и возвращает значение, позволяющее определить, успешно ли выполнено преобразование.
    /// </summary>
    /// 
    /// <returns>
    /// true, если параметр <paramref name="input"/> успешно преобразован, в противном случае — false.
    /// </returns>
    /// <param name="input">Строка, содержащая преобразуемый номер версии.</param><param name="result">После завершения метода содержит объект <see cref="T:System.Version"/>, эквивалентный номеру, содержавшемуся в параметре <paramref name="input"/>, если преобразование выполнено успешно, или объект <see cref="T:System.Version"/>, в котором основной и дополнительный номера версии равны 0, если преобразование завершилось неудачно.</param>
    public static bool TryParse(string input, out Version result)
    {
      Version.VersionResult result1 = new Version.VersionResult();
      result1.Init("input", false);
      bool flag = Version.TryParseVersion(input, ref result1);
      result = result1.m_parsedVersion;
      return flag;
    }

    private static bool TryParseVersion(string version, ref Version.VersionResult result)
    {
      if (version == null)
      {
        result.SetFailure(Version.ParseFailureKind.ArgumentNullException);
        return false;
      }
      else
      {
        string[] strArray = version.Split(new char[1]
        {
          '.'
        });
        int length = strArray.Length;
        if (length < 2 || length > 4)
        {
          result.SetFailure(Version.ParseFailureKind.ArgumentException);
          return false;
        }
        else
        {
          int parsedComponent1;
          int parsedComponent2;
          if (!Version.TryParseComponent(strArray[0], "version", ref result, out parsedComponent1) || !Version.TryParseComponent(strArray[1], "version", ref result, out parsedComponent2))
            return false;
          int num = length - 2;
          if (num > 0)
          {
            int parsedComponent3;
            if (!Version.TryParseComponent(strArray[2], "build", ref result, out parsedComponent3))
              return false;
            if (num - 1 > 0)
            {
              int parsedComponent4;
              if (!Version.TryParseComponent(strArray[3], "revision", ref result, out parsedComponent4))
                return false;
              result.m_parsedVersion = new Version(parsedComponent1, parsedComponent2, parsedComponent3, parsedComponent4);
            }
            else
              result.m_parsedVersion = new Version(parsedComponent1, parsedComponent2, parsedComponent3);
          }
          else
            result.m_parsedVersion = new Version(parsedComponent1, parsedComponent2);
          return true;
        }
      }
    }

    private static bool TryParseComponent(string component, string componentName, ref Version.VersionResult result, out int parsedComponent)
    {
      if (!int.TryParse(component, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out parsedComponent))
      {
        result.SetFailure(Version.ParseFailureKind.FormatException, component);
        return false;
      }
      else
      {
        if (parsedComponent >= 0)
          return true;
        result.SetFailure(Version.ParseFailureKind.ArgumentOutOfRangeException, componentName);
        return false;
      }
    }

    internal enum ParseFailureKind
    {
      ArgumentNullException,
      ArgumentException,
      ArgumentOutOfRangeException,
      FormatException,
    }

    internal struct VersionResult
    {
      internal Version m_parsedVersion;
      internal Version.ParseFailureKind m_failure;
      internal string m_exceptionArgument;
      internal string m_argumentName;
      internal bool m_canThrow;

      internal void Init(string argumentName, bool canThrow)
      {
        this.m_canThrow = canThrow;
        this.m_argumentName = argumentName;
      }

      internal void SetFailure(Version.ParseFailureKind failure)
      {
        this.SetFailure(failure, string.Empty);
      }

      internal void SetFailure(Version.ParseFailureKind failure, string argument)
      {
        this.m_failure = failure;
        this.m_exceptionArgument = argument;
        if (this.m_canThrow)
          throw this.GetVersionParseException();
      }

      internal Exception GetVersionParseException()
      {
        switch (this.m_failure)
        {
          case Version.ParseFailureKind.ArgumentNullException:
            return (Exception) new ArgumentNullException(this.m_argumentName);
          case Version.ParseFailureKind.ArgumentException:
            return (Exception) new ArgumentException(Environment.GetResourceString("Arg_VersionString"));
          case Version.ParseFailureKind.ArgumentOutOfRangeException:
            return (Exception) new ArgumentOutOfRangeException(this.m_exceptionArgument, Environment.GetResourceString("ArgumentOutOfRange_Version"));
          case Version.ParseFailureKind.FormatException:
            try
            {
              int.Parse(this.m_exceptionArgument, (IFormatProvider) CultureInfo.InvariantCulture);
            }
            catch (FormatException ex)
            {
              return (Exception) ex;
            }
            catch (OverflowException ex)
            {
              return (Exception) ex;
            }
            return (Exception) new FormatException(Environment.GetResourceString("Format_InvalidString"));
          default:
            return (Exception) new ArgumentException(Environment.GetResourceString("Arg_VersionString"));
        }
      }
    }
  }
}
