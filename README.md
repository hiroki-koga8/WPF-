# 機能説明
----

# 初期表示
下記の画面を表示して、タスク表示処理を実施する。
<img width="783" height="492" alt="image" src="https://github.com/user-attachments/assets/4e45ab2a-2952-4cf2-86c3-a74f05ea0cc0" />

* 基準日に現在の日付を設定
* 状態は"すべて"を設定
  * 状態の選択肢は下記の通り
  *  <img width="166" height="127" alt="image" src="https://github.com/user-attachments/assets/d7ab6931-5837-4e6c-a3d3-34c7817b29d3" />

# 新規登録ボタン押下時
登録画面を表示する。
<img width="777" height="501" alt="image" src="https://github.com/user-attachments/assets/603e3a48-5477-45f7-bce0-f5475004b292" />

* タスク名に"新規タスク"を設定
* 開始日に現在の日付を設定
* 終了日に現在の日付+1日を設定
* 説明に"説明を入力"を設定
* 状態は"未対応"を選択
  *  状態の選択肢は下記の通り
  *  <img width="341" height="96" alt="image" src="https://github.com/user-attachments/assets/880582df-b069-46ec-8c95-758ebf925b41" />
* 予定時間に1を設定
* 実績時間に0を設定
* 備考に"備考"を設定

### OKボタン押下時

* 登録画面に入力した内容をDBに登録して、画面を終了する。
* タスク管理画面で画面表示処理を実施する。
  
<img width="786" height="492" alt="image" src="https://github.com/user-attachments/assets/68e3ed89-a0f7-42ce-a2c8-e632bceb48a9" />
<img width="769" height="326" alt="image" src="https://github.com/user-attachments/assets/ca274de7-99cc-4e89-b40a-4852f5bc886d" />


### キャンセルボタン押下時

* 登録画面を終了する。

# 編集ボタン押下時
編集ボタンを押したタスクの編集画面を表示する。※タスクを選択していない場合編集ボタンは非活性

<img width="787" height="496" alt="image" src="https://github.com/user-attachments/assets/e2c8c268-d673-439a-b56b-240678c43dbb" />

### OKボタン押下時

* 編集画面に入力した内容をDBで更新して、画面を終了する。
* タスク管理画面でタスク表示処理を実施する。
  
* 編集してOK
   * <img width="785" height="494" alt="image" src="https://github.com/user-attachments/assets/c0c77080-7fde-450f-b521-cab904ced100" />
* 編集内容で更新されている
   * <img width="788" height="495" alt="image" src="https://github.com/user-attachments/assets/baeba977-86c6-47ff-82a5-6f9b58c192bb" />
   * <img width="852" height="331" alt="image" src="https://github.com/user-attachments/assets/c35176b1-8b10-4273-b0fd-ebadcd91975a" />

### キャンセルボタン押下時

* 編集画面を終了する。


# 削除ボタン押下時
削除ボタンを押したタスクを削除する※タスクを選択していない場合編集ボタンは非活性

* 確認メッセージを表示する
   * <img width="784" height="491" alt="image" src="https://github.com/user-attachments/assets/04c06019-22d2-4e69-965a-fb0b94de4e5e" />
* 「はい」を押したとき
  * メッセージが閉じ、タスクが削除される。
  * タスク表示処理を実行する。
  * <img width="786" height="485" alt="image" src="https://github.com/user-attachments/assets/c863660e-ee10-4b30-bd73-db34008f8727" />
  * <img width="691" height="351" alt="image" src="https://github.com/user-attachments/assets/58f7b9e0-4404-431c-9dc5-38bbdd264b93" />
* 「いいえ」をおしたとき、メッセージが閉じて、タスクは削除されない。

# 表示ボタン押下時

タスク表示処理を実施する。
基準日が空の場合、メッセージを表示する。
<img width="785" height="491" alt="image" src="https://github.com/user-attachments/assets/3897d943-8fd5-4f89-9617-68295ed61c54" />

# タスク表示処理
タスク表示処理は、初期表示時、登録、編集、削除の実施後と画面の表示ボタン押下時に実施する。

下記の条件すべてに一致するタスクを画面に表示する。

* 画面の基準日がタスクの開始日から終了日に含まれる
* 画面の状態コンボボックスがすべて以外の場合
   * 画面の状態コンボボックスの値と状態が一致するタスク
 
# Excel出力ボタン押下時

画面に表示しているタスクをExcelファイルに出力する。

<img width="783" height="534" alt="image" src="https://github.com/user-attachments/assets/50505c1b-97f7-4761-8942-b63c68649910" />
<img width="787" height="492" alt="image" src="https://github.com/user-attachments/assets/53be51a6-9888-44ca-bf8b-86cb263e7273" />

↓出力したExcelファイル
[Tasks_20251004172925.xlsx](https://github.com/user-attachments/files/22694634/Tasks_20251004172925.xlsx)


----
# 参考資料
WPF ReactiveProperty勉強用
* https://mseeeen.msen.jp/reactive-property-programming-by-wpf-beginner/
