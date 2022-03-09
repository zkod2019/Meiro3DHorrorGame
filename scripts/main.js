import fs from "fs";
import admin from "firebase-admin";

const serviceAccount = JSON.parse(
  fs.readFileSync("./service-account-key.json")
);

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  storageBucket: "meiro-ip.appspot.com",
});

const firestore = admin.firestore();
const storage = admin.storage();
const questionsRef = firestore.collection("questions");

const saveOptions = { predefinedAcl: "publicRead" };
const iqQuestionImages = await Promise.all(
  fs
    .readdirSync("../Assets/Resources/iq/questions")
    .filter((filename) => filename.toLowerCase().endsWith(".png"))
    .map(async (filename) => {
      const contents = fs.readFileSync(
        `../Assets/Resources/iq/questions/${filename}`
      );
      const bucketFileName = `iq-questions/${filename}`;
      await storage.bucket().file(bucketFileName).save(contents, saveOptions);
      console.log(`debug: uploaded: ${bucketFileName}`);
      return bucketFileName;
    })
);

const mathQuestionImages = await Promise.all(
  fs
    .readdirSync("../Assets/Resources/math/questions")
    .filter((filename) => filename.toLowerCase().endsWith(".png"))
    .map(async (filename) => {
      const contents = fs.readFileSync(
        `../Assets/Resources/math/questions/${filename}`
      );
      let bucketFileName = `math-questions/${filename}`;
      await storage.bucket().file(bucketFileName).save(contents, saveOptions);
      console.log(`debug: uploaded: ${bucketFileName}`);
      return bucketFileName;
    })
);

const readingQuestionImages = await Promise.all(
  fs
    .readdirSync("../Assets/Resources/reading/questions")
    .filter((filename) => filename.toLowerCase().endsWith(".png"))
    .map(async (filename) => {
      const contents = fs.readFileSync(
        `../Assets/Resources/reading/questions/${filename}`
      );
      let bucketFileName = `reading-questions/${filename}`;
      await storage.bucket().file(bucketFileName).save(contents, saveOptions);
      console.log(`debug: uploaded: ${bucketFileName}`);
      return bucketFileName;
    })
);

const puzzleQuestionImages = await Promise.all(
  fs
    .readdirSync("../Assets/Resources/tPuzzles/questions")
    .filter((filename) => filename.toLowerCase().endsWith(".png"))
    .map(async (filename) => {
      const contents = fs.readFileSync(
        `../Assets/Resources/tPuzzles/questions/${filename}`
      );
      let bucketFileName = `puzzle-questions/${filename}`;
      await storage.bucket().file(bucketFileName).save(contents, saveOptions);
      console.log(`debug: uploaded: ${bucketFileName}`);
      return bucketFileName;
    })
);

console.log(iqQuestionImages);

const iqQuestionAnswers = fs
  .readdirSync("../Assets/Resources/iq/answers")
  .filter((filename) => filename.toLowerCase().endsWith(".txt"))
  .map((filename) =>
    fs.readFileSync(`../Assets/Resources/iq/answers/${filename}`).toString()
  );

const mathQuestionAnswers = fs
  .readdirSync("../Assets/Resources/math/answers")
  .filter((filename) => filename.toLowerCase().endsWith(".txt"))
  .map((filename) =>
    fs.readFileSync(`../Assets/Resources/math/answers/${filename}`).toString()
  );

const readingQuestionAnswers = fs
  .readdirSync("../Assets/Resources/reading/answers")
  .filter((filename) => filename.toLowerCase().endsWith(".txt"))
  .map((filename) =>
    fs
      .readFileSync(`../Assets/Resources/reading/answers/${filename}`)
      .toString()
  );

const puzzleQuestionAnswers = fs
  .readdirSync("../Assets/Resources/tPuzzles/answers")
  .filter((filename) => filename.toLowerCase().endsWith(".txt"))
  .map((filename) =>
    fs
      .readFileSync(`../Assets/Resources/tPuzzles/answers/${filename}`)
      .toString()
  );

const iqQuestions = iqQuestionImages.map((questionImg, i) => ({
  question: questionImg,
  answer: iqQuestionAnswers[i],
}));

const readingQuestions = readingQuestionImages.map((questionImg, i) => ({
  question: questionImg,
  answer: readingQuestionAnswers[i],
}));

const mathQuestions = mathQuestionImages.map((questionImg, i) => ({
  question: questionImg,
  answer: mathQuestionAnswers[i],
}));

const puzzleQuestions = puzzleQuestionImages.map((questionImg, i) => ({
  question: questionImg,
  answer: puzzleQuestionAnswers[i],
}));

const allQuestions = [
  ...iqQuestions,
  ...readingQuestions,
  ...mathQuestions,
  ...puzzleQuestions,
];

console.log("all questions: ", allQuestions.length);

for (let question of allQuestions) {
  await questionsRef.add(question);
  console.log("added one question");
}
