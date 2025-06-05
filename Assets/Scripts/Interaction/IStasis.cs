using UnityEngine;
public interface IStasis
{
    void StatisEffectActivate();
    void StatisEffectDeactivate();

    bool IsFreezed { get; }                
    void SetOutlineThickness(float value); 
    void SetColorOutline(Color color, float alpha);

}