﻿using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class GameDataHelpers
    {
        #region Combine Dictionary with KeyValuePair functions
        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, MinMaxFloat> CombineDamages(Dictionary<DamageElement, MinMaxFloat> resultDictionary, KeyValuePair<DamageElement, MinMaxFloat> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, MinMaxFloat>();
            DamageElement damageElement = newEntry.Key;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            MinMaxFloat value = newEntry.Value;
            if (!resultDictionary.ContainsKey(damageElement))
                resultDictionary[damageElement] = value;
            else
                resultDictionary[damageElement] += value;
            return resultDictionary;
        }

        /// <summary>
        /// Combine damage infliction amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineDamageInflictions(Dictionary<DamageElement, float> resultDictionary, KeyValuePair<DamageElement, float> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            DamageElement damageElement = newEntry.Key;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            float value = newEntry.Value;
            if (!resultDictionary.ContainsKey(damageElement))
                resultDictionary[damageElement] = value;
            else
                resultDictionary[damageElement] += value;
            return resultDictionary;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<Attribute, float> CombineAttributes(Dictionary<Attribute, float> resultDictionary, KeyValuePair<Attribute, float> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Attribute, float>();
            if (newEntry.Key != null)
            {
                if (!resultDictionary.ContainsKey(newEntry.Key))
                    resultDictionary[newEntry.Key] = newEntry.Value;
                else
                    resultDictionary[newEntry.Key] += newEntry.Value;
            }
            return resultDictionary;
        }

        /// <summary>
        /// Multiply attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyEntry"></param>
        /// <returns></returns>
        public static Dictionary<Attribute, float> MultiplyAttributes(Dictionary<Attribute, float> resultDictionary, KeyValuePair<Attribute, float> multiplyEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Attribute, float>();
            if (multiplyEntry.Key != null)
            {
                if (resultDictionary.ContainsKey(multiplyEntry.Key))
                    resultDictionary[multiplyEntry.Key] *= multiplyEntry.Value;
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine currency amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<Currency, int> CombineCurrencies(Dictionary<Currency, int> resultDictionary, KeyValuePair<Currency, int> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Currency, int>();
            if (newEntry.Key != null)
            {
                if (!resultDictionary.ContainsKey(newEntry.Key))
                    resultDictionary[newEntry.Key] = newEntry.Value;
                else
                    resultDictionary[newEntry.Key] += newEntry.Value;
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineResistances(Dictionary<DamageElement, float> resultDictionary, KeyValuePair<DamageElement, float> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (newEntry.Key != null)
            {
                if (!resultDictionary.ContainsKey(newEntry.Key))
                    resultDictionary[newEntry.Key] = newEntry.Value;
                else
                    resultDictionary[newEntry.Key] += newEntry.Value;
                if (resultDictionary[newEntry.Key] > newEntry.Key.MaxResistanceAmount)
                    resultDictionary[newEntry.Key] = newEntry.Key.MaxResistanceAmount;
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine armor amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineArmors(Dictionary<DamageElement, float> resultDictionary, KeyValuePair<DamageElement, float> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (newEntry.Key != null)
            {
                if (!resultDictionary.ContainsKey(newEntry.Key))
                    resultDictionary[newEntry.Key] = newEntry.Value;
                else
                    resultDictionary[newEntry.Key] += newEntry.Value;
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine skill levels dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<BaseSkill, short> CombineSkills(Dictionary<BaseSkill, short> resultDictionary, KeyValuePair<BaseSkill, short> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<BaseSkill, short>();
            if (newEntry.Key != null)
            {
                if (!resultDictionary.ContainsKey(newEntry.Key))
                    resultDictionary[newEntry.Key] = newEntry.Value;
                else
                    resultDictionary[newEntry.Key] += newEntry.Value;
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine item amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<BaseItem, short> CombineItems(Dictionary<BaseItem, short> resultDictionary, KeyValuePair<BaseItem, short> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<BaseItem, short>();
            if (newEntry.Key != null)
            {
                if (!resultDictionary.ContainsKey(newEntry.Key))
                    resultDictionary[newEntry.Key] = newEntry.Value;
                else
                    resultDictionary[newEntry.Key] += newEntry.Value;
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine ammo type amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<AmmoType, short> CombineAmmoTypes(Dictionary<AmmoType, short> resultDictionary, KeyValuePair<AmmoType, short> newEntry)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<AmmoType, short>();
            if (newEntry.Key != null)
            {
                if (!resultDictionary.ContainsKey(newEntry.Key))
                    resultDictionary[newEntry.Key] = newEntry.Value;
                else
                    resultDictionary[newEntry.Key] += newEntry.Value;
            }
            return resultDictionary;
        }
        #endregion

        #region Combine Dictionary with Dictionary functions
        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, MinMaxFloat> CombineDamages(Dictionary<DamageElement, MinMaxFloat> resultDictionary, Dictionary<DamageElement, MinMaxFloat> combineDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, MinMaxFloat>();
            if (combineDictionary != null && combineDictionary.Count > 0)
            {
                foreach (KeyValuePair<DamageElement, MinMaxFloat> entry in combineDictionary)
                {
                    CombineDamages(resultDictionary, entry);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine damage infliction amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineDamageInflictions(Dictionary<DamageElement, float> resultDictionary, Dictionary<DamageElement, float> combineDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (combineDictionary != null && combineDictionary.Count > 0)
            {
                foreach (KeyValuePair<DamageElement, float> entry in combineDictionary)
                {
                    CombineDamageInflictions(resultDictionary, entry);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static Dictionary<Attribute, float> CombineAttributes(Dictionary<Attribute, float> resultDictionary, Dictionary<Attribute, float> combineDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Attribute, float>();
            if (combineDictionary != null && combineDictionary.Count > 0)
            {
                foreach (KeyValuePair<Attribute, float> entry in combineDictionary)
                {
                    CombineAttributes(resultDictionary, entry);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Multiply attribute amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="multiplyDictionary"></param>
        /// <returns></returns>
        public static Dictionary<Attribute, float> MultiplyAttributes(Dictionary<Attribute, float> resultDictionary, Dictionary<Attribute, float> multiplyDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Attribute, float>();
            if (multiplyDictionary != null && multiplyDictionary.Count > 0)
            {
                // Remove attributes that are not multiplying
                List<Attribute> availableAttributes = new List<Attribute>(resultDictionary.Keys);
                foreach (Attribute attribute in availableAttributes)
                {
                    if (!multiplyDictionary.ContainsKey(attribute))
                        resultDictionary.Remove(attribute);
                }
                foreach (KeyValuePair<Attribute, float> entry in multiplyDictionary)
                {
                    MultiplyAttributes(resultDictionary, entry);
                }
            }
            else
            {
                resultDictionary.Clear();
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineResistances(Dictionary<DamageElement, float> resultDictionary, Dictionary<DamageElement, float> combineDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (combineDictionary != null && combineDictionary.Count > 0)
            {
                foreach (KeyValuePair<DamageElement, float> entry in combineDictionary)
                {
                    CombineResistances(resultDictionary, entry);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine defend amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineArmors(Dictionary<DamageElement, float> resultDictionary, Dictionary<DamageElement, float> combineDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (combineDictionary != null && combineDictionary.Count > 0)
            {
                foreach (KeyValuePair<DamageElement, float> entry in combineDictionary)
                {
                    CombineArmors(resultDictionary, entry);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine skill levels dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="combineDictionary"></param>
        /// <returns></returns>
        public static Dictionary<BaseSkill, short> CombineSkills(Dictionary<BaseSkill, short> resultDictionary, Dictionary<BaseSkill, short> combineDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<BaseSkill, short>();
            if (combineDictionary != null && combineDictionary.Count > 0)
            {
                foreach (KeyValuePair<BaseSkill, short> entry in combineDictionary)
                {
                    CombineSkills(resultDictionary, entry);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine item amounts dictionary
        /// </summary>
        /// <param name="resultDictionary"></param>
        /// <param name="newEntry"></param>
        /// <returns></returns>
        public static Dictionary<BaseItem, short> CombineItems(Dictionary<BaseItem, short> resultDictionary, Dictionary<BaseItem, short> combineDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<BaseItem, short>();
            if (combineDictionary != null && combineDictionary.Count > 0)
            {
                foreach (KeyValuePair<BaseItem, short> entry in combineDictionary)
                {
                    CombineItems(resultDictionary, entry);
                }
            }
            return resultDictionary;
        }
        #endregion

        #region Make KeyValuePair functions
        /// <summary>
        /// Make damage - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <param name="effectiveness"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, MinMaxFloat> ToKeyValuePair(this DamageAmount source, float rate, float effectiveness)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, MinMaxFloat>(damageElement, (source.amount * rate) + effectiveness);
        }

        /// <summary>
        /// Make damage amount
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <param name="effectiveness"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, MinMaxFloat> ToKeyValuePair(this DamageIncremental source, short level, float rate, float effectiveness)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, MinMaxFloat>(damageElement, (source.amount.GetAmount(level) * rate) + effectiveness);
        }

        /// <summary>
        /// Make damage infliction - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this DamageInflictionAmount source)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.rate);
        }

        /// <summary>
        /// Make damage infliction - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this DamageInflictionIncremental source, short level)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.rate.GetAmount(level));
        }

        /// <summary>
        /// Make attribute - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<Attribute, float> ToKeyValuePair(this AttributeAmount source, float rate)
        {
            if (source.attribute == null)
                return new KeyValuePair<Attribute, float>();
            return new KeyValuePair<Attribute, float>(source.attribute, source.amount * rate);
        }

        /// <summary>
        /// Make attribute - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<Attribute, float> ToKeyValuePair(this AttributeIncremental source, short level, float rate)
        {
            if (source.attribute == null)
                return new KeyValuePair<Attribute, float>();
            return new KeyValuePair<Attribute, float>(source.attribute, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make currency - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<Currency, int> ToKeyValuePair(this CurrencyAmount source)
        {
            return new KeyValuePair<Currency, int>(source.currency, source.amount);
        }

        /// <summary>
        /// Make resistance - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ResistanceAmount source, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount * rate);
        }

        /// <summary>
        /// Make resistance - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ResistanceIncremental source, short level, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make armor - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ArmorAmount source, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount * rate);
        }

        /// <summary>
        /// Make armor - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static KeyValuePair<DamageElement, float> ToKeyValuePair(this ArmorIncremental source, short level, float rate)
        {
            DamageElement damageElement = source.damageElement;
            if (damageElement == null)
                damageElement = GameInstance.Singleton.DefaultDamageElement;
            return new KeyValuePair<DamageElement, float>(damageElement, source.amount.GetAmount(level) * rate);
        }

        /// <summary>
        /// Make skill - level key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseSkill, short> ToKeyValuePair(this SkillLevel source)
        {
            if (source.skill == null)
                return new KeyValuePair<BaseSkill, short>();
            return new KeyValuePair<BaseSkill, short>(source.skill, source.level);
        }

        /// <summary>
        /// Make skill - level key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseSkill, short> ToKeyValuePair(this MonsterSkill source)
        {
            if (source.skill == null)
                return new KeyValuePair<BaseSkill, short>();
            return new KeyValuePair<BaseSkill, short>(source.skill, source.level);
        }

        /// <summary>
        /// Make item - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<BaseItem, short> ToKeyValuePair(this ItemAmount source)
        {
            if (source.item == null)
                return new KeyValuePair<BaseItem, short>();
            return new KeyValuePair<BaseItem, short>(source.item, source.amount);
        }

        /// <summary>
        /// Make ammo type - amount key-value pair
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static KeyValuePair<AmmoType, short> ToKeyValuePair(this AmmoTypeAmount source)
        {
            if (source.ammoType == null)
                return new KeyValuePair<AmmoType, short>();
            return new KeyValuePair<AmmoType, short>(source.ammoType, source.amount);
        }
        #endregion

        #region Combine Dictionary functions
        /// <summary>
        /// Combine damage effectiveness attribute amounts dictionary
        /// </summary>
        /// <param name="sourceEffectivesses"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static Dictionary<Attribute, float> CombineDamageEffectivenessAttributes(DamageEffectivenessAttribute[] sourceEffectivesses, Dictionary<Attribute, float> resultDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Attribute, float>();
            if (sourceEffectivesses != null && sourceEffectivesses.Length > 0)
            {
                foreach (DamageEffectivenessAttribute sourceEffectivess in sourceEffectivesses)
                {
                    if (sourceEffectivess.attribute == null)
                        continue;
                    if (!resultDictionary.ContainsKey(sourceEffectivess.attribute))
                        resultDictionary[sourceEffectivess.attribute] = sourceEffectivess.effectiveness;
                    else
                        resultDictionary[sourceEffectivess.attribute] += sourceEffectivess.effectiveness;
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, MinMaxFloat> CombineDamages(DamageAmount[] sourceAmounts, Dictionary<DamageElement, MinMaxFloat> resultDictionary, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, MinMaxFloat>();
            if (sourceAmounts != null && sourceAmounts.Length > 0)
            {
                KeyValuePair<DamageElement, MinMaxFloat> pair;
                foreach (DamageAmount sourceAmount in sourceAmounts)
                {
                    pair = ToKeyValuePair(sourceAmount, rate, 0f);
                    resultDictionary = CombineDamages(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine damage amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, MinMaxFloat> CombineDamages(DamageIncremental[] sourceIncrementals, Dictionary<DamageElement, MinMaxFloat> resultDictionary, short level, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, MinMaxFloat>();
            if (sourceIncrementals != null && sourceIncrementals.Length > 0)
            {
                KeyValuePair<DamageElement, MinMaxFloat> pair;
                foreach (DamageIncremental sourceIncremental in sourceIncrementals)
                {
                    pair = ToKeyValuePair(sourceIncremental, level, rate, 0f);
                    resultDictionary = CombineDamages(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine damage infliction amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineDamageInflictions(DamageInflictionIncremental[] sourceIncrementals, Dictionary<DamageElement, float> resultDictionary, short level)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (sourceIncrementals != null && sourceIncrementals.Length > 0)
            {
                KeyValuePair<DamageElement, float> pair;
                foreach (DamageInflictionIncremental sourceIncremental in sourceIncrementals)
                {
                    pair = ToKeyValuePair(sourceIncremental, level);
                    resultDictionary = CombineDamageInflictions(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<Attribute, float> CombineAttributes(AttributeAmount[] sourceAmounts, Dictionary<Attribute, float> resultDictionary, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Attribute, float>();
            if (sourceAmounts != null && sourceAmounts.Length > 0)
            {
                KeyValuePair<Attribute, float> pair;
                foreach (AttributeAmount sourceAmount in sourceAmounts)
                {
                    pair = ToKeyValuePair(sourceAmount, rate);
                    resultDictionary = CombineAttributes(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine attribute amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<Attribute, float> CombineAttributes(AttributeIncremental[] sourceIncrementals, Dictionary<Attribute, float> resultDictionary, short level, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Attribute, float>();
            if (sourceIncrementals != null && sourceIncrementals.Length > 0)
            {
                KeyValuePair<Attribute, float> pair;
                foreach (AttributeIncremental sourceIncremental in sourceIncrementals)
                {
                    pair = ToKeyValuePair(sourceIncremental, level, rate);
                    resultDictionary = CombineAttributes(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine currency amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Dictionary<Currency, int> CombineCurrencies(CurrencyAmount[] sourceAmounts, Dictionary<Currency, int> resultDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<Currency, int>();
            if (sourceAmounts != null && sourceAmounts.Length > 0)
            {
                KeyValuePair<Currency, int> pair;
                foreach (CurrencyAmount sourceAmount in sourceAmounts)
                {
                    pair = ToKeyValuePair(sourceAmount);
                    resultDictionary = CombineCurrencies(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineResistances(ResistanceAmount[] sourceAmounts, Dictionary<DamageElement, float> resultDictionary, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (sourceAmounts != null && sourceAmounts.Length > 0)
            {
                KeyValuePair<DamageElement, float> pair;
                foreach (ResistanceAmount sourceAmount in sourceAmounts)
                {
                    pair = ToKeyValuePair(sourceAmount, rate);
                    resultDictionary = CombineResistances(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine resistance amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineResistances(ResistanceIncremental[] sourceIncrementals, Dictionary<DamageElement, float> resultDictionary, short level, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (sourceIncrementals != null && sourceIncrementals.Length > 0)
            {
                KeyValuePair<DamageElement, float> pair;
                foreach (ResistanceIncremental sourceIncremental in sourceIncrementals)
                {
                    pair = ToKeyValuePair(sourceIncremental, level, rate);
                    resultDictionary = CombineResistances(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }


        /// <summary>
        /// Combine armor amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineArmors(ArmorAmount[] sourceAmounts, Dictionary<DamageElement, float> resultDictionary, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (sourceAmounts != null && sourceAmounts.Length > 0)
            {
                KeyValuePair<DamageElement, float> pair;
                foreach (ArmorAmount sourceAmount in sourceAmounts)
                {
                    pair = ToKeyValuePair(sourceAmount, rate);
                    resultDictionary = CombineArmors(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine armor amounts dictionary
        /// </summary>
        /// <param name="sourceIncrementals"></param>
        /// <param name="resultDictionary"></param>
        /// <param name="level"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static Dictionary<DamageElement, float> CombineArmors(ArmorIncremental[] sourceIncrementals, Dictionary<DamageElement, float> resultDictionary, short level, float rate)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<DamageElement, float>();
            if (sourceIncrementals != null && sourceIncrementals.Length > 0)
            {
                KeyValuePair<DamageElement, float> pair;
                foreach (ArmorIncremental sourceIncremental in sourceIncrementals)
                {
                    pair = ToKeyValuePair(sourceIncremental, level, rate);
                    resultDictionary = CombineArmors(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine skill levels dictionary
        /// </summary>
        /// <param name="sourceLevels"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static Dictionary<BaseSkill, short> CombineSkills(SkillLevel[] sourceLevels, Dictionary<BaseSkill, short> resultDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<BaseSkill, short>();
            if (sourceLevels != null && sourceLevels.Length > 0)
            {
                KeyValuePair<BaseSkill, short> pair;
                foreach (SkillLevel sourceLevel in sourceLevels)
                {
                    pair = ToKeyValuePair(sourceLevel);
                    resultDictionary = CombineSkills(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine monster skills dictionary
        /// </summary>
        /// <param name="sourceMonsterSkills"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static Dictionary<BaseSkill, short> CombineSkills(MonsterSkill[] sourceMonsterSkills, Dictionary<BaseSkill, short> resultDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<BaseSkill, short>();
            if (sourceMonsterSkills != null && sourceMonsterSkills.Length > 0)
            {
                KeyValuePair<BaseSkill, short> pair;
                foreach (MonsterSkill sourceMonsterSkill in sourceMonsterSkills)
                {
                    pair = ToKeyValuePair(sourceMonsterSkill);
                    resultDictionary = CombineSkills(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine item amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static Dictionary<BaseItem, short> CombineItems(ItemAmount[] sourceAmounts, Dictionary<BaseItem, short> resultDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<BaseItem, short>();
            if (sourceAmounts != null && sourceAmounts.Length > 0)
            {
                KeyValuePair<BaseItem, short> pair;
                foreach (ItemAmount sourceAmount in sourceAmounts)
                {
                    pair = ToKeyValuePair(sourceAmount);
                    resultDictionary = CombineItems(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }

        /// <summary>
        /// Combine ammo type amounts dictionary
        /// </summary>
        /// <param name="sourceAmounts"></param>
        /// <param name="resultDictionary"></param>
        /// <returns></returns>
        public static Dictionary<AmmoType, short> CombineAmmoTypes(AmmoTypeAmount[] sourceAmounts, Dictionary<AmmoType, short> resultDictionary)
        {
            if (resultDictionary == null)
                resultDictionary = new Dictionary<AmmoType, short>();
            if (sourceAmounts != null && sourceAmounts.Length > 0)
            {
                KeyValuePair<AmmoType, short> pair;
                foreach (AmmoTypeAmount sourceAmount in sourceAmounts)
                {
                    pair = ToKeyValuePair(sourceAmount);
                    resultDictionary = CombineAmmoTypes(resultDictionary, pair);
                }
            }
            return resultDictionary;
        }
        #endregion

        #region Calculate functions
        public static float GetEffectivenessDamage(Dictionary<Attribute, float> effectivenessAttributes, ICharacterData character)
        {
            float damageEffectiveness = 0f;
            if (effectivenessAttributes != null && character != null)
            {
                Dictionary<Attribute, float> characterAttributes = character.GetCaches().Attributes;
                foreach (Attribute attribute in characterAttributes.Keys)
                {
                    if (attribute != null && effectivenessAttributes.ContainsKey(attribute))
                        damageEffectiveness += effectivenessAttributes[attribute] * characterAttributes[attribute];
                }
            }
            return damageEffectiveness;
        }

        public static CharacterStats GetStatsFromAttributes(Dictionary<Attribute, float> attributeAmounts)
        {
            CharacterStats stats = new CharacterStats();
            if (attributeAmounts != null && attributeAmounts.Count > 0)
            {
                foreach (Attribute attribute in attributeAmounts.Keys)
                {
                    if (attribute == null) continue;
                    stats += attribute.statsIncreaseEachLevel * attributeAmounts[attribute];
                }
            }
            return stats;
        }

        public static MinMaxFloat GetSumDamages(Dictionary<DamageElement, MinMaxFloat> damages)
        {
            MinMaxFloat totalDamageAmount = new MinMaxFloat();
            totalDamageAmount.min = 0;
            totalDamageAmount.max = 0;
            if (damages == null || damages.Count == 0)
                return totalDamageAmount;
            foreach (MinMaxFloat damageAmount in damages.Values)
            {
                totalDamageAmount += damageAmount;
            }
            return totalDamageAmount;
        }
        #endregion
    }
}
