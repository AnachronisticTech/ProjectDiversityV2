using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HelperNamespace;
using System;

/// <summary>
///     [What does this Stat do]
/// </summary>
[System.Serializable]
public sealed class Stat
{
    //[SerializeField]
    public StatTypes _statName = StatTypes.Unknown;

    //[SerializeField]
    public float _value = 0.0f;

    //[SerializeField]
    public bool _hasMinValue = false;
    //[SerializeField, ConditionalHide(nameof(_hasMinValue), true)]
    public float _minValue = float.MinValue;

    //[SerializeField]
    public bool _hasMaxValue = false;
    //[SerializeField, ConditionalHide(nameof(_hasMaxValue), true)]
    public float _maxValue = float.MaxValue;

    public bool _showHideStat = true;

    public Stat(StatTypes name, float value, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        _statName = name;
        _value = value;

        if (Values.IsFloatLessThan(minValue, float.MinValue))
        {
            _hasMinValue = true;
        }
        _minValue = minValue;

        if (Values.IsFloatMoreThan(maxValue, float.MaxValue))
        {
            _hasMaxValue = true;
        }
        _maxValue = maxValue;
    }

    public string GetName
    {
        get => Enum.GetName(typeof(StatTypes), _statName);
    }

    public void SetValue(float value)
    {
        _value = value;
    }
    public float GetValue
    {
        get => _value;
    }

    public bool IncreaseValue(float value)
    {
        _value += value;

        if (Values.IsFloatMoreThan(_value, _maxValue))
        {
            _value = _maxValue;

            return true;
        }

        return false;
    }
    public bool DecreaseValue(float value)
    {
        _value -= value;

        if (Values.IsFloatLessThan(_value, _minValue))
        {
            _value = _minValue;

            return true;
        }

        return false;
    }

    public bool HasMaxValue
    {
        get => _hasMaxValue;
    }
    public bool HasMinValue
    {
        get => _hasMinValue;
    }

    public float GetMaxValue
    {
        get => _hasMaxValue ? _maxValue : float.MaxValue;
    }
    public float GetMinValue
    {
        get => _hasMinValue ? _minValue : float.MinValue;
    }

    public void SetMaxValue(float value)
    {
        if (_hasMaxValue)
        {
            _maxValue = value;
        }
        else
        {
            Debug.LogWarning(_statName + " on " + this.ToString() + " does not have a max value.");
            _maxValue = float.MaxValue;
        }
    }
    public void SetMinValue(float value)
    {
        if (_hasMinValue)
        {
            _minValue = value;
        }
        else
        {
            Debug.LogWarning(_statName + " on " + this.ToString() + " does not have a min value.");
            _minValue = float.MinValue;
        }
    }
}
