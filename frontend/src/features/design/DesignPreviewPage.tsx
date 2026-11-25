import { Button } from '../../components/Button'
import { Card } from '../../components/Card'
import type { BadgeVariant } from '../../components/Card'
import { Progress } from '../../components/Progress'

const swatches = [
  'primary',
  'primary-soft',
  'secondary',
  'secondary-soft',
  'accent',
  'accent-soft',
]

const causes: {
  title: string
  badge: string
  badgeVariant: BadgeVariant
  value: number
  max: number
}[] = [
  { title: 'بطاطين الشتاء', badge: 'عاجل', badgeVariant: 'accent', value: 18400, max: 25000 },
  { title: 'مستلزمات المدارس', badge: 'تعليم', badgeVariant: 'secondary', value: 9200, max: 12000 },
  { title: 'سلال الغذاء الشهرية', badge: 'مستمر', badgeVariant: 'primary', value: 41000, max: 60000 },
]

export function DesignPreviewPage() {
  return (
    <>
      <section className="hero">
        <div className="container stack">
          <h1>كل تبرع يصل إلى من يحتاجه</h1>
          <p>صفحة اختبار للتصميم. كل لون وحجم ومسافة هنا مأخوذ من المتغيرات المشتركة.</p>
          <div className="row">
            <Button variant="accent" size="lg">تبرع الآن</Button>
            <Button variant="on-dark" size="lg">كيف يعمل</Button>
          </div>
        </div>
      </section>

      <section className="section">
        <div className="container stack">
          <h2>الألوان</h2>
          <div className="card-grid">
            {swatches.map((name) => (
              <div key={name} className={`swatch swatch--${name}`}>
                {name}
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="section section--alt">
        <div className="container stack">
          <h2>بطاقات الحملات</h2>
          <div className="card-grid">
            {causes.map((cause) => (
              <Card
                key={cause.title}
                title={cause.title}
                badge={cause.badge}
                badgeVariant={cause.badgeVariant}
              >
                <p className="muted">وصف مختصر لوجهة التبرعات.</p>
                <Progress
                  value={cause.value}
                  max={cause.max}
                  label={cause.title}
                  caption={`تم جمع ${cause.value.toLocaleString('ar-EG')}`}
                />
                <Button size="sm">تبرع لهذه الحملة</Button>
              </Card>
            ))}
          </div>
        </div>
      </section>

      <section className="section">
        <div className="container stack">
          <h2>الأزرار والحقول</h2>
          <div className="row">
            <Button>أساسي</Button>
            <Button variant="secondary">ثانوي</Button>
            <Button variant="accent">مميز</Button>
            <Button variant="ghost">محايد</Button>
            <Button disabled>معطل</Button>
          </div>
          <div className="row">
            <Button size="sm">صغير</Button>
            <Button>متوسط</Button>
            <Button size="lg">كبير</Button>
          </div>
          <div className="form-field form-field--narrow">
            <label htmlFor="amount">مبلغ التبرع</label>
            <input id="amount" type="number" placeholder="500" />
          </div>
        </div>
      </section>
    </>
  )
}
