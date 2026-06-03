
using System.Collections;

namespace _MGGTmod.types.models.Custom;

public class KeyMapType<T>: IEnumerable<KeyValuePair<string, T>>
{
    public T MapBigmap { get; set; }
    public T MapShoreline { get; set; }
    public T MapRezervbase { get; set; }
    public T MapFactory4 { get; set; }
    public T MapWoods { get; set; }
    public T MapLighthouse { get; set; }
    public T MapInterchange { get; set; }
    public T MapLaboratory { get; set; }
    public T MapTarkovstreets { get; set; }
    public T MapSandbox { get; set; }
    public T MapLabyrinth { get; set; }
    public T MapUnknown { get; set; }
    
    // 实现 GetEnumerator，返回所有属性名+值
    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        yield return new(nameof(MapBigmap), MapBigmap);
        yield return new(nameof(MapShoreline), MapShoreline);
        yield return new(nameof(MapRezervbase), MapRezervbase);
        yield return new(nameof(MapFactory4), MapFactory4);
        yield return new(nameof(MapWoods), MapWoods);
        yield return new(nameof(MapLighthouse), MapLighthouse);
        yield return new(nameof(MapInterchange), MapInterchange);
        yield return new(nameof(MapLaboratory), MapLaboratory);
        yield return new(nameof(MapTarkovstreets), MapTarkovstreets);
        yield return new(nameof(MapSandbox), MapSandbox);
        yield return new(nameof(MapLabyrinth), MapLabyrinth);
        yield return new(nameof(MapUnknown), MapUnknown);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}