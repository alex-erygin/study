### 18.07.2019

---------------
Test data 1 requred
---------------

Пример поискового запроса по нескольким параметрам. Работает как И.
```js
GET /courses/_search
{
  "query": {
    "bool":{
      "must":[
        {"match": {"name": "computer"}},
        {"match": {"room": "c8"}}  
      ]
    }
  }
}
```

Пример поискового запроса с отрицательным условием. Работает как И.
```js
GET /courses/_search
{
  "query": {
    "bool":{
      "must":[
        {"match": {"name": "accounting"}},
        {"match": {"room": "e3"}}        
      ],

      "must_not":[
          {"match": {"professor.name": "bill"}}
      ]
    }
  }
}
```

Пример запроса с multi_mutch. Работает как ИЛИ.

```js
GET /courses/_search
{
  "query": {
    "multi_match": {
      "fields": ["name", "professor.department"],
      "query": "accounting"
    }
  }
}   
```


Поиск подстроки в поле
```js
GET /courses/_search
{
  "query": {
    "match_phrase": {
      "course_description": "from the business school"
    }
  }
}
```

Запрос на выбор диапазона (>= 19 и <= 20) (и с датами работает)
```js
GET /courses/_search
{
  "query": {
    "range": {
      "students_enrolled": {
        "gte": 19,
        "lte": 20
      }
    }
  }
}
```

Получить список индексов
```js
GET /_cat/indices?v
```

Поиск по подстроке и диапазону дат:
```js
GET /ao-json-*/_search/
{
  "query": {
    "bool":{
      "must":[
        {
          "range": {
            "@timestamp": {
              "gte": "2019-07-01T00:00:00Z",
              "lte": "2019-07-20T00:00:00Z"
              }
            }
        },
        { "match": {"atol.environment": "production"}}
      ]
    }
  }
}
```

-----------------------------
Test data 2 required
-----------------------------

Запрос с поиском и группировкой (найти тачки не красные, сгруппированные по производителю,  для каждого производителя вывести сред, мин и мак цену)
```js
GET /vehicles/cars/_search
{
  "query":{
    "bool":{
      "must_not": [
        { "match":{"color":"red-"} }
      ]
    }
    
  },
  "aggs": {
    "popular_cars": {
      "terms": {
        "field": "make.keyword"
      },
      "aggs": {
        "avg_price": {
          "avg":{
            "field":"price"
          }
        },
        "max_price": {
          "max": {
            "field": "price"
          }
        },
        "min_price": {
          "min":{
            "field":"price"
          }
        }
      }
    }
  }
}
```

Запрос с двойной группировкой. Группируем по состоянию машины, для каждого состояния вычисляется средняя цена, внутри группы с состоянием для каждой марки вычисляется макс, мин и средняя цена.
```js
GET /vehicles/cars/_search
{
  "size": 0, 
  "aggs": {
    "car_conditions": {
      "terms": {
        "field":"condition.keyword"
      },
      "aggs": {
        "avg_price":{
          "avg": {
            "field":"price"
          }
        },
        "make":{
          "terms":{
            "field":"make.keyword"
          },
          "aggs":{
            "min_price":{"min":{"field":"price"}},
            "max_price":{"max":{"field":"price"}},
            "avg_price":{"avg":{"field":"price"}}
          }
          
        }
      }
    }
  }
}
```