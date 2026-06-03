# MG-GT-Mod-CSharp

The MG series Mod: MG-GeneralTrader-Mod

# MG 通用商人框架 — 商人文件撰写说明书

> 对应代码版本：v0.3.0 | SPT 4.0.13 | 最后更新：2026-01-03

---

## 目录

1. [概述](#1-概述)
2. [目录结构总览](#2-目录结构总览)
3. [文件分类与优先级](#3-文件分类与优先级)
4. [必选文件详述](#4-必选文件详述)
   - [4.1 traderInfo.json](#41-traderinfojson)
   - [4.2 {traderId}.jpg 头像](#42-traderidjpg-头像)
   - [4.3 traderData/assort.json](#43-traderdataassortjson)
   - [4.4 globals.json](#44-globalsjson)
5. [可选文件详述](#5-可选文件详述)
   - [5.1 items/*.json](#51-itemsjson)
   - [5.2 locales/itemsdescription.json](#52-localesitemsdescriptionjson)
   - [5.3 locales/mail.json](#53-localesmailjson)
   - [5.4 templates/handbook.json](#54-templateshandbookjson)
   - [5.5 templates/quests.json](#55-templatesquestsjson)
   - [5.6 traderData/questassort.json](#56-traderdataquestassortjson)
   - [5.7 bundles.json](#57-bundlesjson)
   - [5.8 images/quests/ （任务图标）](#58-imagesquests-任务图标)
6. [当前无效或死代码路径](#6-当前无效或死代码路径)
7. [常用类型参考](#7-常用类型参考)
8. [完整示例：FlanrecGents](#8-完整示例flanrecgents)
9. [常见问题](#9-常见问题)

---

## 1. 概述

本 mod 的商人系统通过读取 `./traders/` 目录下**每个子目录**来加载自定义商人。  
每个子目录代表一个商人，目录名任意（建议使用英文/拼音），内部按约定结构放置 JSON 配置文件和资源。

加载入口：`CustomTraderServices.AddTraders()` → 遍历 `./traders/` 下的每个子目录，依次调用以下方法：

```
AddTraderBaseToDB()      ← 加载 traderInfo.json + assort + 头像
AddImageToDB()           ← 加载 images/quests/ 任务图标
AddTraderItemsToDB()     ← 加载 items/*.json 独立物品
AddTraderLocalesToDB()   ← 加载 locales/*.json 翻译/邮件
AddTraderLocationToDB()  ← ⚠️ 空方法，当前不执行任何操作
AddTraderTemplatesToDB() ← 加载 templates/*.json 手册/任务
AddTraderGlobalsToDB()   ← 加载 globals.json Buff/ItemPreset
```

---

## 2. 目录结构总览

```
traders/
├── <TraderName>/                     ← 目录名任意，建议英文
│   ├── {traderId}.jpg                ← 商人头像（文件名必须等于 traderInfo.json 中的 _id）
│   ├── bundles.json                  ← 资源包清单
│   ├── globals.json                  ← 全局 Buff 和 ItemPreset
│   ├── traderInfo.json               ← 【必选】商人核心配置
│   ├── items/                        ← 商人独立物品
│   │   ├── item1.json
│   │   └── item2.json
│   ├── locales/                      ← 本地化文本
│   │   ├── itemsdescription.json     ← 物品名称/描述翻译
│   │   └── mail.json                 ← 任务邮件文本
│   ├── location/                     ← 自定义战利品（⚠️ 当前无效）
│   │   └── looseLoot.json
│   ├── templates/                    ← 手册和任务模板
│   │   ├── handbook.json
│   │   └── quests.json
│   ├── traderData/                   ← 商店数据
│   │   ├── assort.json               ← 商品清单
│   │   └── questassort.json          ← 任务关联
│   └── images/                       ← 图片资源
│       └── quests/                   ← 任务图标（.jpg）
```

---

## 3. 文件分类与优先级

| 优先级 | 文件 | 必选？ | 加载失败影响 |
|--------|------|--------|-------------|
| 🔴 致命 | `traderInfo.json` | **是** | 该商人跳过加载，日志警告 |
| 🔴 致命 | `{traderId}.jpg` | **是** | 商人无头像，日志警告，不终止 |
| 🔴 致命 | `traderData/assort.json` | **是** | 商人不卖任何东西（空商店） |
| 🟡 可选 | `globals.json` | 否 | 无自定义 Buff / 物品预设 |
| 🟡 可选 | `items/*.json` | 否 | 无独立物品 |
| 🟡 可选 | `locales/itemsdescription.json` | 否 | 物品显示默认 ID 名 |
| 🟡 可选 | `locales/mail.json` | 否 | 任务无邮件文本 |
| 🟡 可选 | `templates/handbook.json` | 否 | 物品不在跳蚤手册显示 |
| 🟡 可选 | `templates/quests.json` | 否 | 无自定义任务 |
| 🟡 可选 | `traderData/questassort.json` | 否 | 任务不关联商店物品 |
| ⚪ 可选 | `bundles.json` | 否 | 不使用外部 bundle |
| ⚪ 可选 | `images/quests/*` | 否 | 任务无图标 |
| ⚫ **无效** | `location/looseLoot.json` | — | `AddTraderLocationToDB()` 空方法，写了也不会加载 |

---

## 4. 必选文件详述

### 4.1 traderInfo.json

**对应 C# 类型：** `CustomTraderInfo`  
**源码位置：** `types/models/EFT/traders/CustomTraders.cs` 第 38-52 行

这是商人的**身份证**，没有它该商人会被跳过。

#### 字段表

| JSON 字段 | C# 类型 | Required | 说明 | 示例值 |
|-----------|---------|----------|------|--------|
| `enable` | `bool` | **是** | 设为 `false` 则跳过该商人 | `true` |
| `_id` | `string` | **是** | 唯一 ID，建议 24 位 MongoId 格式 | `"9cc23608f000000000000000"` |
| `name` | `string` | **是** | 商人程序内部名称 | `"FlanrecGents"` |
| `locales` | `object` | **是** | 地域化信息（见下） | |
| `locales.FullName` | `string` | **是** | 商人全名 | `"FG商人"` |
| `locales.FirstName` | `string` | **是** | 商人名字 | `"FG商人"` |
| `locales.Nickname` | `string` | **是** | 商人昵称（任务/界面显示） | `"FG商人"` |
| `locales.Location` | `string` | **是** | 商人所在位置描述 | `"MG实验室"` |
| `locales.Description` | `string` | **是** | 商人背景描述 | `"FG是MG商人的好朋友..."` |
| `insurance` | `object` | 否 | 保险配置 | |
| `insurance.enable` | `bool` | 否 | 是否开启保险 | `true` |
| `insurance.minreturnTime` | `int` | 否 | 最短保险返回时间（小时） | `0` |
| `insurance.maxreturnTime` | `int` | 否 | 最长保险返回时间（小时） | `0` |
| `insurance.pay` | `int` | 否 | 保险费用 | `0` |
| `insurance.chance` | `int` | 否 | 保险返回概率 | `100` |
| `insurance.storageTime` | `int` | 否 | 保险存储时间 | `100` |
| `insurance.Message` | `object` | 否 | 保险对话消息 | 见示例 |
| `repair` | `object` | 否 | 维修配置 | |
| `repair.enable` | `bool` | 否 | 是否开启维修 | `false` |
| `repair.coefficient` | `int` | 否 | 维修价格系数 | `1` |
| `repair.quality` | `int` | 否 | 维修品质 | `3` |
| `loyaltyLevels` | `object` | 否 | 好感等级配置 | |
| `loyaltyLevels.description` | `object` | 否 | 等级说明（实际代码不使用，仅注释用途） | |
| `loyaltyLevels.range` | `array` | 否 | 等级数组（见下） | |
| `loyaltyLevels.range[].minLevel` | `int` | 否 | 解锁最低玩家等级 | `1` |
| `loyaltyLevels.range[].minSalesSum` | `int` | 否 | 解锁最低累计消费 | `0` |
| `loyaltyLevels.range[].minStanding` | `double` | 否 | 解锁最低好感 | `0` |
| `loyaltyLevels.range[].buy_price_coef` | `int` | 否 | 购买价格系数（正变贵，负打折） | `0` |
| `loyaltyLevels.range[].repair_price_coef` | `int` | 否 | 维修折扣系数 | `-50` |
| `loyaltyLevels.range[].insurance_price_coef` | `int` | 否 | 保险折扣系数 | `30` |
| `loyaltyLevels.range[].exchange_price_coef` | `int` | 否 | 交易系数 | `0` |
| `loyaltyLevels.range[].heal_price_coef` | `int` | 否 | 治疗折扣系数 | `100` |
| `discount` | `int` | 否 | 商人折扣（百分比） | `0` |
| `medic` | `bool` | 否 | 是否为医疗商人 | `false` |
| `updateTime` | `object` | 否 | 刷新时间（秒） | |
| `updateTime.min` | `int` | 否 | 最小刷新间隔 | `600` |
| `updateTime.max` | `int` | 否 | 最大刷新间隔 | `600` |
| `unlockedDefault` | `bool` | 否 | 是否默认解锁 | `true` |

#### 完整示例

```json
{
  "enable": true,
  "name": "MyTrader",
  "_id": "9cc23608f000000000000000",
  "locales": {
    "FullName": "我的商人",
    "FirstName": "我的",
    "Nickname": "商人",
    "Location": "藏身处",
    "Description": "一位神秘的商人。"
  },
  "insurance": {
    "enable": true,
    "minreturnTime": 0,
    "maxreturnTime": 0,
    "pay": 0,
    "chance": 100,
    "storageTime": 100,
    "Message": {
      "insuranceStart": ["咳咳，又丢东西了？"],
      "insuranceFound": ["找到了，拿好别又丢了。"],
      "insuranceFailed": ["抱歉，没找回来。"],
      "insuranceExpired": ["时间过了，东西没了。"],
      "insuranceComplete": ["下次小心点。"],
      "insuranceFailedLabs": []
    }
  },
  "repair": { "enable": false, "coefficient": 1, "quality": 3 },
  "loyaltyLevels": {
    "description": { "main": "好感等级说明" },
    "range": [
      { "minLevel": 1, "minSalesSum": 0, "minStanding": 0, "buy_price_coef": 0,
        "repair_price_coef": 0, "insurance_price_coef": 0,
        "exchange_price_coef": 0, "heal_price_coef": 0 }
    ]
  },
  "discount": 0,
  "medic": false,
  "updateTime": { "min": 600, "max": 600 },
  "unlockedDefault": true
}
```

---

### 4.2 {traderId}.jpg 头像

**文件名必须等于 `traderInfo.json` 中的 `_id` 字段值，大小写敏感。**  
例如：`_id: "9cc23608f000000000000000"` → 头像文件 `9cc23608f000000000000000.jpg`

- 放在商人目录根下（`./traders/<TraderName>/`）
- 代码自动注册为头像路由，替换 `base.json` 模板中的默认路径
- 如果缺少该文件，日志会警告但不会终止加载

---

### 4.3 traderData/assort.json

**对应 C# 类型：** `TraderAssortStringId` → 经 `ReplaceKey` 转换后变为 `TraderAssort`  
**源码位置：** `types/models/EFT/traders/CustomAssorts.cs` 第 15-22 行（`TraderAssortStringId`）、第 24-33 行（`ItemString`）

这是商人的**商品货架**，定义了卖什么、价格多少、需要多少好感。

#### 字段表

| JSON 字段 | C# 类型 | Required | 说明 |
|-----------|---------|----------|------|
| `NextResupply` | `double` | 否 | 下次补货时间（通常不填） |
| `items` | `array` | **是** | 商品列表 |
| `items[]._id` | `string` | **是** | 货架项 ID（非物品 ID），可自定义字符串。**如果不符合 MongoId 格式，代码会自动替换为随机 MongoId** |
| `items[]._tpl` | `string` | **是** | 物品模板 ID（游戏中已有的物品 ID） |
| `items[].parentId` | `string` | **是** | 固定为 `"hideout"` |
| `items[].slotId` | `string` | **是** | 固定为 `"hideout"` |
| `items[].upd.UnlimitedCount` | `bool` | 否 | 是否无限库存 | `true` |
| `items[].upd.StackObjectsCount` | `int` | 否 | 库存数量 | `9999999` |
| `barter_scheme` | `object` | **是** | 价格方案，key 为 `items[]._id` |
| `barter_scheme.{itemId}[][].count` | `int` | **是** | 价格数量 |
| `barter_scheme.{itemId}[][]._tpl` | `string` | **是** | 货币 ID（卢布：`5449016a4bdc2d6f028b456f`，美元：`5696686a4bdc2da3298b456a`，欧元：`569668774bdc2da2298b4568`） |
| `loyal_level_items` | `object` | **是** | 所需好感等级，key 为 `items[]._id` |
| `loyal_level_items.{itemId}` | `int` | **是** | 所需等级（1-4） |

#### 完整示例

```json
{
  "items": [
    {
      "_id": "MyTrader_default_assort_item_01",
      "_tpl": "544fb3f34bdc2d03748b456a",
      "parentId": "hideout",
      "slotId": "hideout",
      "upd": {
        "UnlimitedCount": true,
        "StackObjectsCount": 9999999
      }
    }
  ],
  "barter_scheme": {
    "MyTrader_default_assort_item_01": [
      [
        { "count": 50000, "_tpl": "5449016a4bdc2d6f028b456f" }
      ]
    ]
  },
  "loyal_level_items": {
    "MyTrader_default_assort_item_01": 1
  }
}
```

> **注意：** `_id` 字段如果不符合 MongoId 格式（24 位十六进制字符串），代码会自动将其转换为随机生成的 MongoId，并同步更新 `barter_scheme`、`loyal_level_items` 和 `questassort.json` 中的关联 key。所以你可以放心使用人类可读的 ID。

---

### 4.4 globals.json

**对应 C# 类型：** `CustomGlobals`  
**源码位置：** `types/models/EFT/MGGlobals.cs` 第 9-13 行

用于向全局数据库注册自定义 **Buff** 和 **ItemPreset**（物品预设，即改装方案）。

#### 字段表

| JSON 字段 | C# 类型 | Required | 说明 |
|-----------|---------|----------|------|
| `ItemPresets` | `Dictionary<string, Preset>` | 否 | 物品预设（武器改装方案），key 为预设名 |
| `Buffs` | `Dictionary<string, List<Buff>>` | 否 | 物品 Buff，key 为 Buff 名（如 `"Buffs_FG-Endurance1"`） |

#### Buff 结构

| 字段 | 类型 | Required | 说明 | 示例值 |
|------|------|----------|------|--------|
| `AbsoluteValue` | `bool` | 是 | 是否为绝对值 | `true` |
| `BuffType` | `string` | 是 | Buff 类型（见下方） | `"MaxStamina"` |
| `Chance` | `double` | 是 | 触发概率 (0-1) | `1` |
| `Delay` | `double` | 是 | 延迟（秒） | `1` |
| `Duration` | `double` | 是 | 持续时间（秒） | `300` |
| `SkillName` | `string` | 否 | 关联技能名（如 `"Strength"`） | `""` |
| `Value` | `double` | 是 | 数值 | `50` |

**常用 BuffType 值：**
- `MaxStamina` — 最大耐力
- `StaminaRate` — 耐力回复速率
- `SkillRate` — 技能经验速率（需配合 `SkillName`）
- `EnergyRate` — 能量回复速率
- `HydrationRate` — 水分回复速率
- `HealthRate` — 生命回复速率
- `RemoveAllBloodLosses` — 移除所有流血
- `StomachBloodloss` — 胃部流血

#### 完整示例

```json
{
  "Buffs": {
    "Buffs_MyStim": [
      {
        "AbsoluteValue": true,
        "BuffType": "MaxStamina",
        "Chance": 1,
        "Delay": 1,
        "Duration": 300,
        "SkillName": "",
        "Value": 50
      }
    ]
  }
}
```

---

## 5. 可选文件详述

### 5.1 items/*.json

**对应 C# 类型：** `CustomTraderItems`  
**源码位置：** `types/models/EFT/templetes/CustomItems.cs` 第 58-63 行

向游戏添加**全新的物品**（通过克隆现有物品模板）。  
每个 JSON 文件对应一个物品，放在 `items/` 目录下。

#### 字段表

| JSON 字段 | C# 类型 | Required | 说明 |
|-----------|---------|----------|------|
| `item` | `TemplateItem` | **是** | SPT 标准物品定义（`_id`、`_name`、`_parent`、`_type`、`_props`、`_proto`） |
| `item._id` | `string` | **是** | 物品 ID，建议 MongoId 格式 |
| `item._name` | `string` | **是** | 物品程序内部名称 |
| `item._parent` | `string` | **是** | 父级分类 ID（如 `"5448f3a64bdc2d60728b456a"` = 注射器） |
| `item._type` | `string` | **是** | 物品类型，通常为 `"Item"` |
| `item._props` | `object` | **是** | SPT 物品属性（取决于物品类型） |
| `item._proto` | `string` | **是** | 克隆自哪个已有物品的模板 ID |
| `origin` | `string` | 否 | 来源物品 ID（用于过滤器继承）。如果填写且为合法 MongoId，会自动将该物品加入其父容器的 Filters 白名单 |
| `type` | `array` | 否 | 类型标签（目前代码中未使用） |

#### 示例（注射器类物品）

```json
{
  "item": {
    "_id": "9cc236080000000000000000",
    "_name": "MySpecialStim",
    "_parent": "5448f3a64bdc2d60728b456a",
    "_type": "Item",
    "_props": {
      "Name": "特效针",
      "ShortName": "特效针",
      "Description": "一支特效针。",
      "Weight": 0.05,
      "BackgroundColor": "orange",
      "Width": 1,
      "Height": 1,
      "StackMaxSize": 1,
      "ItemSound": "med_stimulator",
      "Prefab": {
        "path": "assets/content/weapons/usable_items/item_syringe/item_stimulator_sj6_loot.bundle",
        "rcid": ""
      },
      "UsePrefab": {
        "path": "assets/content/weapons/usable_items/item_syringe/item_stimulator_sj6_container.bundle",
        "rcid": ""
      },
      "StimulatorBuffs": "Buffs_MyStim",
      "medUseTime": 2,
      "medEffectType": "duringUse",
      "MaxHpResource": 20,
      "hpResourceRate": 0,
      "effects_health": [],
      "effects_damage": {}
    },
    "_proto": "544fb3f34bdc2d03748b456a"
  },
  "origin": "5c0e531d86f7747fa23f4d42",
  "type": []
}
```

> **说明：** `_props` 的内容完全取决于物品类型（武器、护甲、注射器、容器等各有不同属性集），请参考 SPT 官方物品格式。`_proto` 指向一个游戏中已有的物品模板 ID，新物品会继承其全部属性，再以 `_props` 覆盖。

---

### 5.2 locales/itemsdescription.json

**对应 C# 类型：** `Dictionary<MongoId, ItemsDesc>`  
**源码位置：** `types/models/EFT/locales/CustomInfoType.cs` 第 25-30 行

为物品提供本地化名称和描述。

#### 字段表

| JSON 字段 | C# 类型 | Required | 说明 |
|-----------|---------|----------|------|
| `{itemId}` | `object` | **是** | Key 为物品 `_id`（MongoId），值为物品描述 |
| `{itemId}.Name` | `string` | **是** | 物品名称 |
| `{itemId}.ShortName` | `string` | **是** | 物品简称 |
| `{itemId}.Description` | `string` | **是** | 物品描述 |

#### 示例

```json
{
  "9cc236080000000000000000": {
    "Name": "特效耐力针",
    "ShortName": "耐力针",
    "Description": "大幅提升耐力的注射剂。"
  },
  "9cc236080000000000000001": {
    "Name": "特效止血剂",
    "ShortName": "止血剂",
    "Description": "效果极好的止血剂。"
  }
}
```

---

### 5.3 locales/mail.json

**对应 C# 类型：** `Dictionary<MongoId, QuestDesc>`  
**源码位置：** `types/models/EFT/locales/CustomInfoType.cs` 第 39-51 行

为自定义任务提供邮件文本（任务接取/完成/失败时的消息）。

#### 字段表

| JSON 字段 | C# 类型 | Required | 说明 |
|-----------|---------|----------|------|
| `{questId}` | `object` | **是** | Key 为任务 ID |
| `{questId}.name` | `string` | **是** | 任务名称 |
| `{questId}.description` | `string` | **是** | 任务描述 |
| `{questId}.startedMessageText` | `string` | 否 | 任务开始时的消息 |
| `{questId}.successMessageText` | `string` | 否 | 任务成功消息 |
| `{questId}.failMessageText` | `string` | 否 | 任务失败消息 |
| `{questId}.changeQuestMessageText` | `string` | 否 | 任务变更消息 |
| `{questId}.acceptPlayerMessage` | `string` | 否 | 接受任务时玩家收到消息 |
| `{questId}.completePlayerMessage` | `string` | 否 | 完成任务时玩家消息 |
| `{questId}.declinePlayerMessage` | `string` | 否 | 拒绝任务时玩家消息 |
| `{questId}.other` | `object` | 否 | 其他自定义键值对（`Dictionary<MongoId, string>`） |

---

### 5.4 templates/handbook.json

**对应 C# 类型：** `List<HandbookItem>`  
**源码位置：** SPTarkov 内置类型

将物品添加到跳蚤市场手册分类。

#### 字段表

| JSON 字段 | 类型 | Required | 说明 |
|-----------|------|----------|------|
| `Id` | `string` | **是** | 物品 ID（必须与 items/*.json 中的 `_id` 一致） |
| `ParentId` | `string` | **是** | 手册父分类 ID（如 `"5b47574386f77428ca22b33a"` = 药物） |
| `Price` | `int` | **是** | 跳蚤市场价格 |

#### 示例

```json
[
  {
    "Id": "9cc236080000000000000000",
    "ParentId": "5b47574386f77428ca22b33a",
    "Price": 50000
  }
]
```

---

### 5.5 templates/quests.json

**对应 C# 类型：** `Dictionary<MongoId, Quest>`  
**源码位置：** SPTarkov 内置类型

添加自定义任务。

#### 示例结构

```json
{
  "{questId}": {
    "_id": "{questId}",
    "traderId": "{traderId}",
    ...  // 标准 SPT Quest 对象
  }
}
```

> 任务对象结构复杂（包含 conditions、stages、rewards 等），请参考 SPT 官方任务格式。空字典也是合法的（`{}`）。

---

### 5.6 traderData/questassort.json

**对应 C# 类型：** `Dictionary<string, Dictionary<string, string>>`  
**源码位置：** `CustomTraderServices.cs` 第 190-194 行

将任务与商店物品关联起来（完成任务后解锁该物品购买权限）。

#### 字段表

| JSON 字段 | 类型 | Required | 说明 |
|-----------|------|----------|------|
| `started` | `object` | **是** | 任务开始后解锁的物品，key=任务ID, value=assort物品`_id` |
| `success` | `object` | **是** | 任务成功后解锁的物品 |
| `fail` | `object` | **是** | 任务失败后解锁的物品 |

#### 示例

```json
{
  "started": {},
  "success": {},
  "fail": {}
}
```

---

### 5.7 bundles.json

**对应 C# 类型：** `BundleManifest`  
**源码位置：** SPTarkov 内置 `BundleManifest`

如果需要使用自定义 Unity AssetBundle（如自定义模型、贴图），在此声明。  
若不使用外部 bundle，保持空清单即可。

```json
{
  "manifest": []
}
```

---

### 5.8 images/quests/ 任务图标

**加载方法：** `CustomTraderServices.AddImageToDB()`  
**源码位置：** `CustomTraderServices.cs` 第 267-280 行

将 `images/quests/` 目录下所有文件注册为任务图标路由。文件扩展名会被自动去除并作为路由 key。

---

## 6. 当前无效或死代码路径

以下路径在代码中定义但**当前不执行任何操作**，即使写了也不会生效。保留它们不会报错，但没有效果。

| 路径/类 | 原因 |
|---------|------|
| `location/looseLoot.json` | `AddTraderLocationToDB()` 方法体为空 |
| `CustomTraderLooseLoot` / `CustomSpawnpoint` / `CustomSpawnpointTemplate` / `CustomSptLootItem` | 对应的 C# 模型零引用，均为死代码 |
| `CustomTraderLocales` | 仅被 `CustomTraders` 引用，而 `CustomTraders` 类本身零引用 |
| `CustomTraderLocation` | 同上 |
| `CustomTraderTemplates` | 同上 |
| `CustomTraderLoyaltyLevelsDesc` | `description` 字段虽被 `CustomTraderLoyaltyLevels` 引用，但从无读取端 |
| `loyaltyLevels.description` 中的各字段（`main`、`minLevel` 等） | 这些字段是字符串类型的"注释"，代码不使用 |

---

## 7. 常用类型参考

### MongoId

- 24 位十六进制字符串（0-9a-f）
- 示例：`"9cc23608f000000000000000"`
- 代码中 `MongoId.IsValidMongoId()` 会做合法性校验，不符合会给出警告
- **注：** 如果你的服务器安装了"无视 MongoId 限制"的模组，可忽略此警告

### 货币 ID

| 货币 | ID |
|------|----|
| 卢布 (RUB) | `5449016a4bdc2d6f028b456f` |
| 美元 (USD) | `5696686a4bdc2da3298b456a` |
| 欧元 (EUR) | `569668774bdc2da2298b4568` |

### 常用物品父级分类 ID

| 分类 | ID |
|------|----|
| 注射器 | `5448f3a64bdc2d60728b456a` |
| 药品 | `5448f3ac4bdc2dce718b4569` |
| 武器 | `5422acb9af1c889c16000029` |
| 弹药 | `5485a8684bdc2da71d8b4567` |
| 配件 | `55818ad54bdc2ddc698b4569` |
| 容器 | `5448bf274bdc2dfc2f8b456a` |

### Handbook 常用父分类 ID

| 分类 | ID |
|------|----|
| 药物/ stims | `5b47574386f77428ca22b33a` |
| 武器 | `5b47574386f77428ca22b2ed` |
| 弹药 | `5b47574386f77428ca22b2f4` |

---

## 8. 完整示例：FlanrecGents

以下为项目自带的示例商人 `FlanrecGents` 的文件清单及加载流程：

### 目录结构

```
traders/FlanrecGents/
├── 9cc23608f000000000000000.jpg          ← 商人头像
├── bundles.json                           ← 空({"manifest":[]})
├── globals.json                           ← 定义 Buffs_FG-Endurance1 和 Buffs_FGZagustin
├── traderInfo.json                        ← 商人核心配置
├── items/
│   ├── FG-Endurance1.json                 ← 耐力针物品
│   └── FG-Zagustin.json                   ← 止血剂物品
├── locales/
│   ├── itemsdescription.json              ← 两个物品的中文名称/描述
│   └── mail.json                          ← 空({})
├── location/
│   └── looseLoot.json                     ← 空({})，当前无效
├── templates/
│   ├── handbook.json                      ← 两个物品的手册条目
│   └── quests.json                        ← 空({})
└── traderData/
    ├── assort.json                        ← 两个物品上架售卖
    └── questassort.json                   ← 空任务关联({})
```

### 加载流程

1. **AddTraderBaseToDB** → 读取 `traderInfo.json` → 检查 enable & _id 合法性 → 加载 `res/services/TraderDB/base.json` 模板 → 用 traderInfo 字段填充 → 读取 dialogue 模板、questassort、assort → 注册头像路由 → 添加商人到数据库
2. **AddImageToDB** → 检查 `images/quests/` 目录（FlanrecGents 没有，跳过）
3. **AddTraderItemsToDB** → 读取 `items/FG-Endurance1.json`、`items/FG-Zagustin.json` → 逐个检查 ID 冲突 → 添加到物品数据库 → 更新 Filters
4. **AddTraderLocalesToDB** → 读取 `locales/itemsdescription.json`（两个物品的中文名）、`locales/mail.json`（空）→ 注册到 LocalesServer
5. **AddTraderLocationToDB** → 空方法，无操作
6. **AddTraderTemplatesToDB** → 读取 `templates/handbook.json` 加入手册 → 读取 `templates/quests.json`（空）→ 无操作
7. **AddTraderGlobalsToDB** → 读取 `globals.json` → 注册 Buffs 和 ItemPresets
8. 加载 `bundles.json`，如有非空 Manifest 则合并到主 bundles.json

---

## 9. 常见问题

### Q: 商人加载失败日志 "不存在配置文件 traderInfo.json"

检查商人目录是否正确，`traderInfo.json` 文件名是否完全一致（大小写敏感）。

### Q: "Id 已存在于游戏中"

商人的 `_id` 与游戏中已有商人 ID 冲突。使用未使用的 MongoId。

### Q: "不符合 MongoId 格式"

`_id` 不是 24 位十六进制字符串。如果使用了非标准 ID，确保已安装解除 MongoId 限制的模组。

### Q: "混蛋，你把我的头像放哪了"

头像文件不存在或文件名不匹配。确保文件名 = `{_id}.jpg` 并放在商人根目录。

### Q: 物品显示为 ID 而不是名称

没有提供 `locales/itemsdescription.json`，或文件中的 key 与物品 `_id` 不匹配。

### Q: 物品添加到游戏但在商店中不显示

检查 `traderData/assort.json` 中的 `_tpl` 是否正确指向物品的 `_id`，以及 `barter_scheme` 和 `loyal_level_items` 的 key 是否与 `items[]._id` 对应。

### Q: 自定义 Buff 不生效

确认 `globals.json` 中的 Buff 名与物品 `_props.StimulatorBuffs` 一致（如 `"StimulatorBuffs": "Buffs_FG-Endurance1"`）。

---

> **提示：** 如果要新建商人，最简单的方式是复制 `traders/FlanrecGents/` 整个目录，然后：
> 1. 修改 `traderInfo.json` 中的 `_id`、`name`、`locales`
> 2. 将头像文件重命名为新的 `_id`
> 3. 替换/修改 `items/` 下的物品 JSON
> 4. 更新 `assort.json` 中的物品引用
> 5. 更新 `globals.json` 中的 Buff 名
