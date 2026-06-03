import { Header } from "@/widgets/header"
import { SidePanelPersonalMenu } from "@/widgets/side-panel-personal-menu"

const PersonalLayout = ({ children }: { children: React.ReactNode }) => {
    return (
        <div className="flex flex-col flex-1 min-h-0 w-full">
            <Header />
            <main className="flex flex-1 min-h-0 my-0 mx-auto w-full overflow-auto">
                <SidePanelPersonalMenu />
                <div className="flex w-full min-h-0">{children}</div>
            </main>
        </div>
    )
}

export default PersonalLayout;