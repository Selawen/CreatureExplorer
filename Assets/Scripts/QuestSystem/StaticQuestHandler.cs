public static class StaticQuestHandler
{
    public delegate void QuestStatusHandler();
    public delegate void QuestPictureHandler(PagePicture picture);

    public static QuestStatusHandler OnQuestOpened;
    public static QuestStatusHandler OnQuestClosed;

    public static QuestStatusHandler OnQuestCompleted;
    public static QuestStatusHandler OnQuestFailed;

    public static QuestPictureHandler OnPictureClicked;
    public static QuestPictureHandler OnPictureDisplayed;

    public static TitanStatue CurrentQuestStatue;

}
