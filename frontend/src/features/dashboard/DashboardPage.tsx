import { Card } from '../../components/Card'
import { Progress } from '../../components/Progress'

export function DashboardPage() {
  return (
    <section className="page">
      <div className="page-heading">
        <h1>لوحة التحكم</h1>
        <p className="muted">صفحة رئيسية كنموذج للواجهة.</p>
      </div>

      <div className="card-grid">
        <Card title="بطاطين الشتاء" badge="عاجل" badgeVariant="accent">
          <p className="muted">وصف مختصر لوجهة التبرعات.</p>
          <Progress value={18400} max={25000} label="بطاطين الشتاء" caption="تم جمع 18,400" />
        </Card>
        <Card title="مستلزمات المدارس" badge="تعليم" badgeVariant="secondary">
          <p className="muted">وصف مختصر لوجهة التبرعات.</p>
          <Progress value={9200} max={12000} label="مستلزمات المدارس" caption="تم جمع 9,200" />
        </Card>
        <Card title="سلال الغذاء الشهرية" badge="مستمر" badgeVariant="primary">
          <p className="muted">وصف مختصر لوجهة التبرعات.</p>
          <Progress value={41000} max={60000} label="سلال الغذاء الشهرية" caption="تم جمع 41,000" />
        </Card>
      </div>
    </section>
  )
}
