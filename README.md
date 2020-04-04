
[![MIT License](http://img.shields.io/badge/license-MIT-blue.svg?style=flat)](LICENSE)
[![BCH compliance](https://bettercodehub.com/edge/badge/KatanoShingo/AutoBackup?branch=master)](https://bettercodehub.com/)  
*AutoBackup*
====

UnitySceneを自動でバックアップを定期的にするアセット

## 📖概要
Unityのクラッシュや上書きにて、現在のシーン状態を飛ばさない用にバックアップ用のSceneデータを別で保存するアセットです。    
[テラシュールブログ](http://tsubakit1.hateblo.jp/entry/20140131/1391094449)にて公開されているAutoSaveがUnity2018では動作しない為、修正を加えて変更をかけました。
AutoSaveの方では、定期的にSceneをSaveしてくれる要素がメインでしたが意図しない状態のSceneが上書きされてしまう為少し前の状態に戻れませんでした。  
バックアップの機能がおまけで付いて居たので、こちらを拡張し定期的にバックアップSceneを別フォルダに保存し不足の事態でも損失が少なくなるようにしました。

## 💃Demo
![バックアップ](https://user-images.githubusercontent.com/40855834/78421648-384c8e00-7694-11ea-83f9-bd382a5194f8.gif)

## 💻要件
Unity2018.4.15f1にて作成

## 🏃使い方
- unitypackageをインポートします。
- メニューバーから`Edit` > `Preferences...`をクリック
- `Preferences Window`が出てくるので、 `Auto Backup`を選択
- auto backupにチェックを入れて、項目設定で動作

| 項目 | 説明 |
| --- | --- |
| auto backup | バックアップを動作させるか |
| Interval | 保存する間隔(分) |  
| quantity | 保存する個数 |  

- `File > Backup > Backup`にて任意のタイミングでもバックアップ可能です
- バックアップしたシーンファイルはBackupフォルダに保存されます

## 🎁unitypackage
[AutoBackup.unitypackage](https://github.com/KatanoShingo/AutoBackup/releases)

## 🔓ライセンス

[MIT](https://github.com/KatanoShingo/AutoBackup/blob/master/LICENSE)

## 🐦著者
[@shi_k_7](https://twitter.com/shi_k_7)
