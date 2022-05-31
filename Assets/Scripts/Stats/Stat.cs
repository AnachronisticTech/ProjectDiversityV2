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
    [SerializeField]
    private StatTypes _statName = StatTypes.Unknown;

    [SerializeField]
    private float _value = 0.0f;

    [SerializeField]
    private bool _hasMinValue = false;
    [SerializeField, ConditionalHide(nameof(_hasMinValue), true)]
    private float _minValue = float.MinValue;

    [SerializeField]
    private bool _hasMaxValue = false;
    [SerializeField, ConditionalHide(nameof(_hasMaxValue), true)]
    private float _maxValue = float.MaxValue;

    public Stat(StatTypes name, float value, float minValue = float.MinValue, float maxValue = float.MaxValue)
    {
        _statName = name;
        _value = value;

        if (ValueCheck.IsFloatLessThan(minValue, float.MinValue))
        {
            _hasMinValue = true;
        }
        _minValue = minValue;

        if (ValueCheck.IsFloatMoreThan(maxValue, float.MaxValue))
        {
            _hasMaxValue = true;
        }
        _maxValue = maxValue;
    }

    public string GetName()
    {
        return Enum.GetName(typeof(StatTypes), _statName);
    }

    public void SetValue(float value)
    {
        _value = value;
    }
    public float GetValue()
    {
        return _value;
    }

    public bool IncreaseValue(float value)
    {
        _value += value;

        if (ValueCheck.IsFloatMoreThan(_value, _maxValue))
        {
            _value = _maxValue;

            return true;
        }

        return false;
    }
    public bool DecreaseValue(float value)
    {
        _value -= value;

        if (ValueCheck.IsFloatLessThan(_value, _minValue))
        {
            _value = _minValue;

            return true;
        }

        return false;
    }

    public bool HasMaxValue()
    {
        return _hasMaxValue;
    }
    public bool HasMinValue()
    {
        return _hasMinValue;
    }

    public float GetMaxValue()
    {
        return _hasMaxValue ? _maxValue : float.MaxValue;
    }
    public float GetMinValue()
    {
        return _hasMinValue ? _minValue : float.MinValue;
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
