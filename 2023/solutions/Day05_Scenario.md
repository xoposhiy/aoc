﻿В сложных предметных областях с большими и запутанными задачами 
становится критически важно умение писать чистый код. 

Сегодня мы рассмотрим несколько концепций, которые помогут в этом:
декомпозиция сверху вниз.

А делать мы это будем на примере задач из AoC
Рассказать, что такое AoC и почему хорошо

Призвать в чатик и подписаться на канал.

5 декабря нам предложили такую задачу. Объясняю задачу.
Нужна картинка для объяснения.

Чтобы лучше понять условие, посмотрим на визуализацию
https://www.reddit.com/media?url=https%3A%2F%2Fpreview.redd.it%2Fcskyqqf8pj4c1.gif%3Fformat%3Dmp4%26s%3D25e72799237f581a4c68c00ad854cb31b0fb1130

Задача сложная и часто это порождает сложный код.
Можно открыть reddit, на котором люди делятся своими решениями
и посмотрим на несколько примеров. 
Все они вызывают страх и не желание разбираться.

Можно подумать, что у сложных задач всегда пугающе сложные решения, но это не так.
Давайте поупражняемся делать понятные решения.

Пора начинать писать код.

Первое правило: в результате декомпозиции должны появляться сущности 
и функции, в соответствующие словам, которые вы произносите во время 
понятного объяснения решения.

Я упоминал такие слова для сущностей:
* Диапазон номеров
* Отображение диапазона
* Этап трансформации
* Альманах

И такие слова для действий: 
* Применение трансформации
* Применение отображения к диапазону
* Пересечение диапазонов


В ООП парадигме сущности превращаются в классы, а действия - в методы этих классов.

В реальной разработке эти слова обычно рождаются из постановки задачи,
спецификации или даже из речи заказчика или будущих пользователей.
Важно, чтобы у разработчику, который будет читать этот код через год,
эти слова были знакомы и понятны. Именно это будет помогать ему читать код.

Давайте набросаем в коде наши классы и основные методы и проговорим взаимосвязи между ними.

GetMinOutputNumber. 
Aggregate для концепции последовательного применения операции.
Применять стопку трансформаций к диапазону или ко всем диапазонам сразу?

Лучше ко всем сразу, потому что после первой же трансформации один диапазон 
может разбиться на несколько и все равно придется применять к нескольким.

Transformation.Apply(Range)
Тут кажется, что пора придумывать итеративный алгоритм, 
перебирая все диапазоны и отслеживая какой эффект дает пересечение с каждым из них.
Ещё одно правило чистого кода − итеративных алгоритмов с состоянием лучше избегать 
в пользу декларативных, у которых нет изменяемого состояния.
Разбираем, как это сделать.


Range

Все самые сложные методы оказались в классе Range.
Реализуя их можно уже не помнить всей сложной постановки задачи с ее нюансами.

К сожалению тут не получилось хорошо избавиться от итеративного алгоритма.
Но мы сделали его максимально простым и не страшным.

Давайте обсудим результат!
Слова из нашей устной речи помогли нам сделать декомпозицию на понятные кусочки.

Наше решение не самое быстрое и не самое короткое. 
Наверняка те страшные решения с реддита короче нашего решения.
Но оно больше не страшное. Почему?

1. Маленький размер необходимого контекста внимания - все функции мелкие.
2. Знакомые термины, которые делают весь код локально понятным. 
Не нужно подглядывать в другие функции, чтобы понять текущую.
3. Отсутствие итеративных алгоритмов с изменяемым состоянием упрощает понимание.

Есть ещё одна особенность у этого решения, делающая его более хорошим.
Вся сложность оказалась в классах, максимально отвязанных от исходной задачи. 
А это значит, что у самого сложного кода велика вероятность повторного использования 
в будущем в других задачах.
Обычно декомпозиция сама по себе не дает этого свойства. 
И хороший разработчик из множества возможных вариантов декомпозиции 
предпочитает тот, который даст больше независимых примитивов,
потенциально полезных в будущем.

 








